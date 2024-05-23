using LazyCache;

namespace ProductApi.Application;

public interface IProductStatusCacheService
{
    Task<Dictionary<int, string>> GetStatusesAsync();
}

public class ProductStatusCacheService: IProductStatusCacheService
{
    private readonly IAppCache _cache;
    private static readonly string CacheKey = "ProductStatuses";
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);

    public ProductStatusCacheService(IAppCache cache)
    {
        _cache = cache;
    }

    public Task<Dictionary<int, string>> GetStatusesAsync() =>
        _cache.GetOrAddAsync(CacheKey, LoadStatusesAsync, CacheDuration);

    private Task<Dictionary<int, string>> LoadStatusesAsync()
    {
        var statuses = new Dictionary<int, string>
            {
                { 1, "Active" },
                { 0, "Inactive" }
            };

        return Task.FromResult(statuses);
    }
}
