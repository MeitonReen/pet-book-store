using BookStore.Base.Implementations.__Caching.Helpers;
using BookStore.Base.Implementations.__Caching.Invalidation.Implementation;
using BookStore.Base.Implementations.__Caching.Invalidation.Interface;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using StackExchange.Redis;

namespace BookStore.Base.Implementations.__Caching.ConfigureServices
{
    public static class RedisCacheServiceCollectionExtensions
    {
        public static void AddServerSideRedisCache(this IServiceCollection services,
            Action<ConfigurationOptions> configure)
        {
            services.AddScoped<SpecialMetadataEndpoint>();

            var conf = new ConfigurationOptions();
            configure(conf);

            services.TryAddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer
                .Connect(conf));
            services.AddScoped<ICacheInvalidator, RedisCacheInvalidator>();
        }
    }
}