using Microsoft.AspNetCore.Mvc;
using QAD_User_Review.Services;
using QAD_User_Review.ViewModels;

namespace QAD_User_Review.Controllers
{
    [FeatureAuthorize("Import")]
    public class DataIngestionController : Controller
    {
        private readonly IDataIngestionService _ingestionService;
        private readonly IPermissionService _permissionService;

        public DataIngestionController(
            IDataIngestionService ingestionService,
            IPermissionService permissionService)
        {
            _ingestionService = ingestionService;
            _permissionService = permissionService;
        }

        public async Task<IActionResult> Index()
        {
            string currentUser = GetCurrentUserId();
            string? role = await _permissionService.GetUserRoleCodeAsync(currentUser);
            if (role != "SuperAdmin")
                return RedirectToAction("AccessDenied", "Home");

            return View(new DataIngestionViewModel());
        }

        [HttpPost]
        [RequestSizeLimit(100_000_000)]
        public async Task<IActionResult> UploadSingleFile(IFormFile file, string fileKey)
        {
            string currentUser = GetCurrentUserId();
            string? role = await _permissionService.GetUserRoleCodeAsync(currentUser);
            if (role != "SuperAdmin")
                return Unauthorized();

            if (file == null || file.Length == 0)
                return Json(new { success = false, messages = new[] { "No file selected." } });

            var result = await _ingestionService.LoadSingleFileAsync(file, fileKey);

            return Json(new
            {
                success = result.Success,
                messages = result.Messages,
                duration = result.Duration.TotalSeconds
            });
        }

        [HttpPost]
        public async Task<IActionResult> RunTransform([FromBody] RunTransformRequest? request = null)
        {
            string currentUser = GetCurrentUserId();
            string? role = await _permissionService.GetUserRoleCodeAsync(currentUser);
            if (role != "SuperAdmin")
                return Unauthorized();

            DateTime? lastUpdateDate = null;
            if (!string.IsNullOrWhiteSpace(request?.LastUpdateDate) && DateTime.TryParse(request.LastUpdateDate, out var parsed))
                lastUpdateDate = parsed;

            var result = await _ingestionService.RunTransformAsync(lastUpdateDate);

            return Json(new
            {
                success = result.Success,
                messages = result.Messages,
                duration = result.Duration.TotalSeconds
            });
        }

        private string GetCurrentUserId()
        {
            var parts = User.Identity!.Name!.ToLower().Split('\\');
            return parts.Length > 1 ? parts[1] : parts[0];
        }
    }
}
