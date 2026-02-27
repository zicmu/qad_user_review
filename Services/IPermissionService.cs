namespace QAD_User_Review.Services
{
    public interface IPermissionService
    {
        Task<bool> HasFeatureAsync(string adUsername, string featureCode);
        Task<bool> CanReviewAllAsync(string adUsername);
        Task<string?> GetUserRoleCodeAsync(string adUsername);
        Task<int?> GetEmployeeKeyAsync(string adUsername);
        void ClearCache(string adUsername);
    }
}
