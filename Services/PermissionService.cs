using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using QAD_User_Review.Data;

namespace QAD_User_Review.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly UserReviewContext _context;
        private readonly IMemoryCache _cache;
        private readonly ILogger<PermissionService> _logger;
        private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(15);
        private const string TablesExistKey = "rbac_tables_exist";

        public PermissionService(UserReviewContext context, IMemoryCache cache, ILogger<PermissionService> logger)
        {
            _context = context;
            _cache = cache;
            _logger = logger;
        }

        public async Task<bool> HasFeatureAsync(string adUsername, string featureCode)
        {
            if (!await TablesExistAsync()) return false;
            var permissions = await GetPermissionsAsync(adUsername);
            return permissions.Contains(featureCode);
        }

        public async Task<bool> CanReviewAllAsync(string adUsername)
        {
            return await HasFeatureAsync(adUsername, "ReviewAll");
        }

        public async Task<string?> GetUserRoleCodeAsync(string adUsername)
        {
            if (!await TablesExistAsync()) return null;

            string cacheKey = $"role_{adUsername.ToLower()}";
            if (_cache.TryGetValue(cacheKey, out string? cached))
                return cached;

            var roleCode = await _context.AppUsers
                .Where(au => au.Employee.AD_Username.ToLower() == adUsername.ToLower()
                          && au.IsActive)
                .Select(au => au.AppRole.RoleCode)
                .FirstOrDefaultAsync();

            _cache.Set(cacheKey, roleCode, CacheDuration);
            return roleCode;
        }

        public async Task<int?> GetEmployeeKeyAsync(string adUsername)
        {
            string cacheKey = $"empkey_{adUsername.ToLower()}";
            if (_cache.TryGetValue(cacheKey, out int? cached))
                return cached;

            var key = await _context.DimEmployees
                .Where(e => e.AD_Username.ToLower() == adUsername.ToLower() && e.IsActive)
                .Select(e => (int?)e.EmployeeKey)
                .FirstOrDefaultAsync();

            _cache.Set(cacheKey, key, CacheDuration);
            return key;
        }

        public void ClearCache(string adUsername)
        {
            string lower = adUsername.ToLower();
            _cache.Remove($"permissions_{lower}");
            _cache.Remove($"role_{lower}");
            _cache.Remove($"empkey_{lower}");
        }

        private async Task<bool> TablesExistAsync()
        {
            if (_cache.TryGetValue(TablesExistKey, out bool exists))
                return exists;

            try
            {
                await _context.AppUsers.Take(1).CountAsync();
                _cache.Set(TablesExistKey, true, TimeSpan.FromMinutes(30));
                return true;
            }
            catch (SqlException)
            {
                _logger.LogWarning("RBAC tables not found in database. Run the migration script to create App_User, Ref_AppRole, Ref_AppFeature, and Bridge_RoleFeature tables.");
                _cache.Set(TablesExistKey, false, TimeSpan.FromMinutes(5));
                return false;
            }
        }

        private async Task<HashSet<string>> GetPermissionsAsync(string adUsername)
        {
            string cacheKey = $"permissions_{adUsername.ToLower()}";
            if (_cache.TryGetValue(cacheKey, out HashSet<string>? cached) && cached != null)
                return cached;

            var features = await (
                from au in _context.AppUsers
                join brf in _context.BridgeRoleFeatures on au.AppRoleKey equals brf.AppRoleKey
                join f in _context.RefAppFeatures on brf.FeatureKey equals f.FeatureKey
                where au.Employee.AD_Username.ToLower() == adUsername.ToLower()
                   && au.IsActive
                   && brf.CanAccess
                select f.FeatureCode
            ).ToListAsync();

            var set = new HashSet<string>(features, StringComparer.OrdinalIgnoreCase);
            _cache.Set(cacheKey, set, CacheDuration);
            return set;
        }
    }
}
