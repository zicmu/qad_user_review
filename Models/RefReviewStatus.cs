namespace QAD_User_Review.Models
{
    public class RefReviewStatus
    {
        public int StatusKey { get; set; }
        public string StatusCode { get; set; } = string.Empty;
        public string? StatusDescription { get; set; }

        public ICollection<FactRoleReview> Reviews { get; set; } = new List<FactRoleReview>();
    }
}
