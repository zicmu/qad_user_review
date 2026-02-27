namespace QAD_User_Review.Models
{
    public class DimReviewPeriod
    {
        public int ReviewPeriodKey { get; set; }
        public string PeriodName { get; set; } = string.Empty;
        public string PeriodType { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; } = true;

        public ICollection<FactRoleReview> Reviews { get; set; } = new List<FactRoleReview>();
    }
}
