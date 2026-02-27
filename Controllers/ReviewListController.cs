using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QAD_User_Review.Data;
using QAD_User_Review.Models;
using QAD_User_Review.Services;
using QAD_User_Review.ViewModels;

namespace QAD_User_Review.Controllers
{
    public class ReviewListController : Controller
    {
        private readonly UserReviewContext _context;
        private readonly IEmailService _emailService;

        public ReviewListController(UserReviewContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public async Task<IActionResult> Index()
        {
            string currentUserId = GetCurrentUserId();
            currentUserId = ResolveManagerUserId(currentUserId);

            var reviewer = await _context.DimEmployees
                .FirstOrDefaultAsync(e => e.AD_Username.ToLower() == currentUserId && e.IsActive);

            if (reviewer == null)
            {
                TempData["SuccessMessage"] = "You are not authorized to access the review list.\r\nPlease contact the administrator.";
                return RedirectToAction("Index", "Home");
            }

            bool isReviewer = await _context.BridgeOrgCharts
                .AnyAsync(o => o.ReviewerKey == reviewer.EmployeeKey && o.IsActive);

            if (!isReviewer)
            {
                TempData["SuccessMessage"] = "You are not authorized to access the review list.\r\nPlease contact the administrator.";
                return RedirectToAction("Index", "Home");
            }

            var activePeriod = await _context.DimReviewPeriods
                .FirstOrDefaultAsync(p => p.IsActive);

            if (activePeriod == null)
            {
                TempData["SuccessMessage"] = "No active review period found.\r\nPlease contact the administrator.";
                return RedirectToAction("Index", "Home");
            }

            var statuses = await _context.RefReviewStatuses.ToListAsync();
            var statusLookup = statuses.ToDictionary(s => s.StatusKey, s => s.StatusCode);

            var reviewItems = await _context.FactRoleReviews
                .Include(r => r.Assignment)
                    .ThenInclude(a => a.SystemIdentity)
                        .ThenInclude(si => si.Employee)
                .Include(r => r.Assignment)
                    .ThenInclude(a => a.Role)
                .Include(r => r.Status)
                .Where(r => r.ReviewerKey == reviewer.EmployeeKey
                         && r.ReviewPeriodKey == activePeriod.ReviewPeriodKey)
                .OrderBy(r => r.Assignment.SystemIdentity.Employee.FullName)
                    .ThenBy(r => r.Assignment.Role.RoleCode)
                .ToListAsync();

            var viewItems = reviewItems.Select(r => new ReviewItemViewModel
            {
                ReviewKey = r.ReviewKey,
                EmployeeName = r.Assignment.SystemIdentity.Employee.FullName,
                AD_Username = r.Assignment.SystemIdentity.Employee.AD_Username,
                Plant = r.Assignment.SystemIdentity.Plant,
                SourceSystem = r.Assignment.SourceSystem,
                RoleCode = r.Assignment.Role.RoleCode,
                RoleDescription = r.Assignment.Role.RoleDescription,
                StatusCode = r.Status?.StatusCode ?? "Pending",
                ReviewerComment = r.ReviewerComment,
                RoleKey = r.Assignment.RoleKey
            }).ToList();

            var managedEmployeeKeys = await _context.BridgeOrgCharts
                .Where(o => o.ReviewerKey == reviewer.EmployeeKey && o.IsActive)
                .Select(o => o.EmployeeKey)
                .Distinct()
                .ToListAsync();

            var employees = await _context.DimEmployees
                .Where(e => managedEmployeeKeys.Contains(e.EmployeeKey) && e.IsActive)
                .OrderBy(e => e.FullName)
                .ToListAsync();

            var plants = viewItems
                .Where(v => v.Plant != null)
                .Select(v => v.Plant!)
                .Distinct()
                .OrderBy(p => p)
                .Select(p => new SelectListItem { Text = p, Value = p });

            var employeeList = employees
                .Select(e => new SelectListItem { Text = e.FullName, Value = e.AD_Username });

            var statusList = statuses
                .Select(s => new SelectListItem { Text = s.StatusCode, Value = s.StatusCode });

            var viewModel = new ReviewListMainViewModel
            {
                ReviewItems = viewItems,
                Employees = employeeList,
                Plants = plants,
                Statuses = statusList,
                ReviewerName = reviewer.FullName,
                ActivePeriodName = activePeriod.PeriodName,
                SelectedEmployee = null,
                SelectedStatus = null,
                SelectedPlant = null
            };

            return View("ReviewList", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Save(ReviewListMainViewModel model)
        {
            string currentUserId = GetCurrentUserId();
            int changeCount = 0;

            var statuses = await _context.RefReviewStatuses.ToListAsync();
            var statusCodeToKey = statuses.ToDictionary(s => s.StatusCode, s => s.StatusKey);

            var reviewKeys = model.ReviewItems.Select(x => x.ReviewKey).ToList();
            var existingReviews = await _context.FactRoleReviews
                .Where(r => reviewKeys.Contains(r.ReviewKey))
                .ToListAsync();

            foreach (var existing in existingReviews)
            {
                var submitted = model.ReviewItems.First(r => r.ReviewKey == existing.ReviewKey);

                int? newStatusKey = statusCodeToKey.GetValueOrDefault(submitted.StatusCode);
                string? newComment = submitted.ReviewerComment;

                if (existing.StatusKey != newStatusKey || existing.ReviewerComment != newComment)
                {
                    existing.StatusKey = newStatusKey;
                    existing.ReviewerComment = newComment;
                    existing.ReviewedAt = DateTime.UtcNow;
                    existing.LastModifiedAt = DateTime.UtcNow;
                    existing.UpdatedAt = DateTime.UtcNow;
                    changeCount++;
                }
            }

            await _context.SaveChangesAsync();

            string message = "Data has been successfully saved!\r\n";
            message += changeCount == 0
                ? "No changes made."
                : $"Number of changes made: {changeCount}.";

            TempData["SuccessMessage"] = message;

            int pendingStatusKey = statusCodeToKey.GetValueOrDefault("Pending");
            bool allResolved = !await _context.FactRoleReviews
                .AnyAsync(r => r.StatusKey == null || r.StatusKey == pendingStatusKey);

            if (allResolved)
            {
                try
                {
                    _emailService.SendEmail(
                        "igor.dabic@essexfurukawa.com",
                        "QAD User Review - All Cases Resolved",
                        "<html><body><p>All review cases have been resolved.</p></body></html>");
                }
                catch (Exception)
                {
                    TempData["SuccessMessage"] += "\r\nNote: Email notification could not be sent.";
                }
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> MenuDetails(int roleKey)
        {
            if (roleKey <= 0)
                return BadRequest("Valid role key is required.");

            var role = await _context.DimRoles.FindAsync(roleKey);

            var menuItems = await _context.DimMenus
                .Where(m => m.RoleKey == roleKey)
                .OrderBy(m => m.MenuCode)
                .ToListAsync();

            var viewModel = new MenuDetailViewModel
            {
                MenuItems = menuItems,
                SelectedRole = role?.RoleDescription ?? role?.RoleCode ?? "Unknown"
            };

            return PartialView("MenuDetails", viewModel);
        }

        private string GetCurrentUserId()
        {
            return User.Identity!.Name!.ToLower().Split('\\')[1];
        }

        /// <summary>
        /// Temporary delegation mapping for testing/dev purposes.
        /// TODO: Replace with a proper delegation table in the database.
        /// </summary>
        private static string ResolveManagerUserId(string userId)
        {
            return userId switch
            {
                "edabici" or "epetkoz" => "emarjas",
                _ => userId
            };
        }
    }
}
