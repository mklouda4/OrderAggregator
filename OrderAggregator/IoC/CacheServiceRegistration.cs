using OrderAggregator.Services.Cache;
using OrderAggregator.Services.Interfaces;

namespace OrderAggregator.IoC
{
    public static class CacheServiceRegistration
    {
        public static IServiceCollection AddCacheServices(this IServiceCollection services, IDistributedCacheCfg configuration)
        {
            _ = services.AddSingleton(configuration);
            _ = services.AddDistributedMemoryCache();
            _ = services.AddSingleton<ICacheService, CacheService>();
            return services;
        }
    }
}
