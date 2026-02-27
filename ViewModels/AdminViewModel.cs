using Microsoft.AspNetCore.Mvc.Rendering;

namespace QAD_User_Review.ViewModels
{
    public class AdminViewModel
    {
        public List<AppUserViewModel> AppUsers { get; set; } = new();
        public List<RolePermissionViewModel> RolePermissions { get; set; } = new();
        public IEnumerable<SelectListItem> AvailableRoles { get; set; } = Enumerable.Empty<SelectListItem>();
        public IEnumerable<SelectListItem> AvailableEmployees { get; set; } = Enumerable.Empty<SelectListItem>();
        public bool CanManageRoles { get; set; }
    }

    public class AppUserViewModel
    {
        public int AppUserKey { get; set; }
        public int EmployeeKey { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string AD_Username { get; set; } = string.Empty;
        public int AppRoleKey { get; set; }
        public string RoleCode { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public string AssignedByName { get; set; } = string.Empty;
    }

    public class RolePermissionViewModel
    {
        public int AppRoleKey { get; set; }
        public string RoleCode { get; set; } = string.Empty;
        public string? RoleDescription { get; set; }
        public List<FeaturePermission> Features { get; set; } = new();
    }

    public class FeaturePermission
    {
        public int RoleFeatureKey { get; set; }
        public int FeatureKey { get; set; }
        public string FeatureCode { get; set; } = string.Empty;
        public string? FeatureDescription { get; set; }
        public bool CanAccess { get; set; }
    }

    public class AddUserRequest
    {
        public int EmployeeKey { get; set; }
        public int AppRoleKey { get; set; }
    }

    public class ToggleUserRequest
    {
        public int AppUserKey { get; set; }
    }

    public class UpdatePermissionRequest
    {
        public int RoleFeatureKey { get; set; }
        public bool CanAccess { get; set; }
    }

    public class RemoveUserRequest
    {
        public int AppUserKey { get; set; }
    }
}
