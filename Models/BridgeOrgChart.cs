namespace QAD_User_Review.Models
{
    public class BridgeOrgChart
    {
        public int OrgChartKey { get; set; }
        public int EmployeeKey { get; set; }
        public int ReviewerKey { get; set; }
        public string SourceSystem { get; set; } = string.Empty;
        public DateTime ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime ETL_LoadDate { get; set; }

        public DimEmployee Employee { get; set; } = null!;
        public DimEmployee Reviewer { get; set; } = null!;
    }
}
