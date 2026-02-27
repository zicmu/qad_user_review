using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QAD_User_Review.Data;
using QAD_User_Review.Services;
using QAD_User_Review.ViewModels;

namespace QAD_User_Review.Controllers
{
    [FeatureAuthorize("ManageUsers")]
    public class AdminController : Controller
    {
        private readonly UserReviewContext _context;
        private readonly IPermissionService _permissionService;

        public AdminController(UserReviewContext context, IPermissionService permissionService)
        {
            _context = context;
            _permissionService = permissionService;
        }

        public async Task<IActionResult> Index()
        {
            string currentUser = GetCurrentUserId();
            bool canManageRoles = await _permissionService.HasFeatureAsync(currentUser, "ManageRoles");

            var appUsers = await _context.AppUsers
                .Include(au => au.Employee)
                .Include(au => au.AppRole)
                .Include(au => au.AssignedByEmployee)
                .OrderBy(au => au.Employee.FullName)
                .Select(au => new AppUserViewModel
                {
                    AppUserKey = au.AppUserKey,
                    EmployeeKey = au.EmployeeKey,
                    EmployeeName = au.Employee.FullName,
                    AD_Username = au.Employee.AD_Username,
                    AppRoleKey = au.AppRoleKey,
                    RoleCode = au.AppRole.RoleCode,
                    IsActive = au.IsActive,
                    CreatedAt = au.CreatedAt,
                    AssignedByName = au.AssignedByEmployee.FullName
                })
                .ToListAsync();

            var rolePermissions = new List<RolePermissionViewModel>();
            if (canManageRoles)
            {
                var roles = await _context.RefAppRoles
                    .Include(r => r.RoleFeatures)
                        .ThenInclude(rf => rf.Feature)
                    .OrderBy(r => r.AppRoleKey)
                    .ToListAsync();

                rolePermissions = roles.Select(r => new RolePermissionViewModel
                {
                    AppRoleKey = r.AppRoleKey,
                    RoleCode = r.RoleCode,
                    RoleDescription = r.RoleDescription,
                    Features = r.RoleFeatures
                        .OrderBy(rf => rf.Feature.FeatureCode)
                        .Select(rf => new FeaturePermission
                        {
                            RoleFeatureKey = rf.RoleFeatureKey,
                            FeatureKey = rf.FeatureKey,
                            FeatureCode = rf.Feature.FeatureCode,
                            FeatureDescription = rf.Feature.FeatureDescription,
                            CanAccess = rf.CanAccess
                        }).ToList()
                }).ToList();
            }

            var existingEmployeeKeys = await _context.AppUsers
                .Where(au => au.IsActive)
                .Select(au => au.EmployeeKey)
                .ToListAsync();

            var availableEmployees = await _context.DimEmployees
                .Where(e => e.IsActive && !existingEmployeeKeys.Contains(e.EmployeeKey))
                .OrderBy(e => e.FullName)
                .Select(e => new SelectListItem { Text = e.FullName, Value = e.EmployeeKey.ToString() })
                .ToListAsync();

            var availableRoles = await _context.RefAppRoles
                .OrderBy(r => r.AppRoleKey)
                .Select(r => new SelectListItem { Text = r.RoleCode, Value = r.AppRoleKey.ToString() })
                .ToListAsync();

            var viewModel = new AdminViewModel
            {
                AppUsers = appUsers,
                RolePermissions = rolePermissions,
                AvailableRoles = availableRoles,
                AvailableEmployees = availableEmployees,
                CanManageRoles = canManageRoles
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddUser(AddUserRequest request)
        {
            string currentUser = GetCurrentUserId();
            var currentEmpKey = await _permissionService.GetEmployeeKeyAsync(currentUser);
            if (currentEmpKey == null)
                return BadRequest();

            var existing = await _context.AppUsers
                .FirstOrDefaultAsync(au => au.EmployeeKey == request.EmployeeKey && au.IsActive);
            if (existing != null)
            {
                TempData["SuccessMessage"] = "User already has an active role assignment.";
                return RedirectToAction("Index");
            }

            var appUser = new Models.AppUser
            {
                EmployeeKey = request.EmployeeKey,
                AppRoleKey = request.AppRoleKey,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                AssignedBy = currentEmpKey.Value
            };

            _context.AppUsers.Add(appUser);
            await _context.SaveChangesAsync();

            _permissionService.ClearCache(
                (await _context.DimEmployees.FindAsync(request.EmployeeKey))?.AD_Username ?? "");

            TempData["SuccessMessage"] = "User added successfully.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> ToggleUser(ToggleUserRequest request)
        {
            var appUser = await _context.AppUsers
                .Include(au => au.Employee)
                .FirstOrDefaultAsync(au => au.AppUserKey == request.AppUserKey);
            if (appUser == null)
                return NotFound();

            appUser.IsActive = !appUser.IsActive;
            appUser.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            _permissionService.ClearCache(appUser.Employee.AD_Username);

            TempData["SuccessMessage"] = appUser.IsActive
                ? "User activated successfully."
                : "User deactivated successfully.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [FeatureAuthorize("ManageRoles")]
        public async Task<IActionResult> UpdatePermission(UpdatePermissionRequest request)
        {
            var roleFeature = await _context.BridgeRoleFeatures
                .FirstOrDefaultAsync(rf => rf.RoleFeatureKey == request.RoleFeatureKey);
            if (roleFeature == null)
                return NotFound();

            roleFeature.CanAccess = request.CanAccess;
            await _context.SaveChangesAsync();

            // Clear cache for all users with this role
            var affectedUsernames = await _context.AppUsers
                .Where(au => au.AppRoleKey == roleFeature.AppRoleKey && au.IsActive)
                .Select(au => au.Employee.AD_Username)
                .ToListAsync();

            foreach (var username in affectedUsernames)
                _permissionService.ClearCache(username);

            TempData["SuccessMessage"] = "Permission updated successfully.";
            return RedirectToAction("Index");
        }

        private string GetCurrentUserId()
        {
            var parts = User.Identity!.Name!.ToLower().Split('\\');
            return parts.Length > 1 ? parts[1] : parts[0];
        }
    }
}
