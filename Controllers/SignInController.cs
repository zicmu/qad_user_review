using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QAD_User_Review.Data;

namespace QAD_User_Review.Controllers
{
    public class SignInController : Controller
    {
        private readonly UserReviewContext _context;

        public SignInController(UserReviewContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var reviewerKeys = await _context.BridgeOrgCharts
                .Where(o => o.IsActive)
                .Select(o => o.ReviewerKey)
                .Distinct()
                .ToListAsync();

            var reviewers = await _context.DimEmployees
                .Where(e => reviewerKeys.Contains(e.EmployeeKey) && e.IsActive)
                .OrderBy(e => e.FullName)
                .ToListAsync();

            return View("SignIn", reviewers);
        }
    }
}
