namespace QAD_User_Review.Models
{
    public class FactRoleReview
    {
        public int ReviewKey { get; set; }
        public int ReviewPeriodKey { get; set; }
        public int AssignmentKey { get; set; }
        public int ReviewerKey { get; set; }
        public int? StatusKey { get; set; }
        public string? ReviewerComment { get; set; }
        public DateTime? ReviewedAt { get; set; }
        public DateTime? LastModifiedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public DimReviewPeriod ReviewPeriod { get; set; } = null!;
        public FactUserRoleAssignment Assignment { get; set; } = null!;
        public DimEmployee Reviewer { get; set; } = null!;
        public RefReviewStatus? Status { get; set; }
    }
}
