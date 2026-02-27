namespace QAD_User_Review.Models
{
    public class BridgeRoleFeature
    {
        public int RoleFeatureKey { get; set; }
        public int AppRoleKey { get; set; }
        public int FeatureKey { get; set; }
        public bool CanAccess { get; set; } = true;

        public RefAppRole AppRole { get; set; } = null!;
        public RefAppFeature Feature { get; set; } = null!;
    }
}
