namespace QAD_User_Review.Models
{
    public class AppUser
    {
        public int AppUserKey { get; set; }
        public int EmployeeKey { get; set; }
        public int AppRoleKey { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int AssignedBy { get; set; }

        public DimEmployee Employee { get; set; } = null!;
        public RefAppRole AppRole { get; set; } = null!;
        public DimEmployee AssignedByEmployee { get; set; } = null!;
    }
}
