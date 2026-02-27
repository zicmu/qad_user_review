namespace QAD_User_Review.Models
{
    public class RefAppFeature
    {
        public int FeatureKey { get; set; }
        public string FeatureCode { get; set; } = string.Empty;
        public string? FeatureDescription { get; set; }

        public ICollection<BridgeRoleFeature> RoleFeatures { get; set; } = new List<BridgeRoleFeature>();
    }
}
