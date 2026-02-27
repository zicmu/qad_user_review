namespace QAD_User_Review.Models
{
    public class FactUserRoleAssignment
    {
        public int AssignmentKey { get; set; }
        public int SystemIdentityKey { get; set; }
        public int RoleKey { get; set; }
        public string SourceSystem { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime ETL_LoadDate { get; set; }

        public DimSystemIdentity SystemIdentity { get; set; } = null!;
        public DimRole Role { get; set; } = null!;
        public ICollection<FactRoleReview> Reviews { get; set; } = new List<FactRoleReview>();
    }
}
