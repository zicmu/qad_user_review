namespace QAD_User_Review.Models
{
    public class DimSystemIdentity
    {
        public int SystemIdentityKey { get; set; }
        public int EmployeeKey { get; set; }
        public string SystemUsername { get; set; } = string.Empty;
        public string SourceSystem { get; set; } = string.Empty;
        public string? Plant { get; set; }
        public string? UserType { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime ETL_LoadDate { get; set; }

        public DimEmployee Employee { get; set; } = null!;
        public ICollection<FactUserRoleAssignment> RoleAssignments { get; set; } = new List<FactUserRoleAssignment>();
    }
}
