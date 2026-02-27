namespace QAD_User_Review.Models
{
    public class DimEmployee
    {
        public int EmployeeKey { get; set; }
        public string AD_Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Country { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public ICollection<DimSystemIdentity> SystemIdentities { get; set; } = new List<DimSystemIdentity>();
        public ICollection<BridgeOrgChart> ManagedEmployees { get; set; } = new List<BridgeOrgChart>();
        public ICollection<BridgeOrgChart> ReviewerFor { get; set; } = new List<BridgeOrgChart>();
        public ICollection<FactRoleReview> Reviews { get; set; } = new List<FactRoleReview>();
        public ICollection<AppUser> AppUserAccounts { get; set; } = new List<AppUser>();
        public ICollection<AppUser> AppUserAssignments { get; set; } = new List<AppUser>();
    }
}
