using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace QAD_User_Review.Services
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class FeatureAuthorizeAttribute : Attribute, IAsyncAuthorizationFilter
    {
        public string FeatureCode { get; }

        public FeatureAuthorizeAttribute(string featureCode)
        {
            FeatureCode = featureCode;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;
            if (user.Identity == null || !user.Identity.IsAuthenticated)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var permissionService = context.HttpContext.RequestServices.GetRequiredService<IPermissionService>();
            string adUsername = ExtractUsername(user.Identity.Name!);

            bool hasAccess = await permissionService.HasFeatureAsync(adUsername, FeatureCode);
            if (!hasAccess)
            {
                context.Result = new RedirectToActionResult("AccessDenied", "Home", null);
            }
        }

        private static string ExtractUsername(string identityName)
        {
            var parts = identityName.ToLower().Split('\\');
            return parts.Length > 1 ? parts[1] : parts[0];
        }
    }
}
