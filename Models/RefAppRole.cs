namespace QAD_User_Review.Models
{
    public class RefAppRole
    {
        public int AppRoleKey { get; set; }
        public string RoleCode { get; set; } = string.Empty;
        public string? RoleDescription { get; set; }

        public ICollection<BridgeRoleFeature> RoleFeatures { get; set; } = new List<BridgeRoleFeature>();
        public ICollection<AppUser> AppUsers { get; set; } = new List<AppUser>();
    }
}
