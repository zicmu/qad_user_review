namespace QAD_User_Review.Models
{
    public class DimMenu
    {
        public int MenuKey { get; set; }
        public int RoleKey { get; set; }
        public string MenuCode { get; set; } = string.Empty;
        public string? MenuDescription { get; set; }

        public DimRole Role { get; set; } = null!;
    }
}
