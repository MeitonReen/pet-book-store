using Microsoft.AspNetCore.Builder;

namespace BookStore.Base.Implementations.__Caching.ConfigureRequestPipeline
{
    public static class CacheExtensions
    {
        public static IApplicationBuilder UsePublicEndpointsCache(this IApplicationBuilder app)
        {
            if (app == default)
            {
                throw new ArgumentNullException(nameof(app));
            }

            return app.UseMiddleware<PublicEndpointsCacheMiddleware>();
        }

        public static IApplicationBuilder UsePrivateEndpointsCache(this IApplicationBuilder app)
        {
            if (app == default)
            {
                throw new ArgumentNullException(nameof(app));
            }

            return app.UseMiddleware<PrivateEndpointsCacheMiddleware>();
        }
    }
}