namespace QAD_User_Review.Models
{
    public class DimRole
    {
        public int RoleKey { get; set; }
        public string RoleCode { get; set; } = string.Empty;
        public string? RoleDescription { get; set; }
        public string SourceSystem { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;

        public ICollection<DimMenu> Menus { get; set; } = new List<DimMenu>();
        public ICollection<FactUserRoleAssignment> RoleAssignments { get; set; } = new List<FactUserRoleAssignment>();
    }
}
