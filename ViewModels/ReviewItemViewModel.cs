namespace QAD_User_Review.ViewModels
{
    public class ReviewItemViewModel
    {
        public int ReviewKey { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string AD_Username { get; set; } = string.Empty;
        public string? Plant { get; set; }
        public string SourceSystem { get; set; } = string.Empty;
        public string RoleCode { get; set; } = string.Empty;
        public string? RoleDescription { get; set; }
        public string StatusCode { get; set; } = "Pending";
        public string? ReviewerComment { get; set; }
        public int RoleKey { get; set; }
    }
}
