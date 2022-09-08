using BookStore.Base.Implementations.__Caching.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using StackExchange.Redis;

namespace BookStore.Base.Implementations.__Caching.ConfigureRequestPipeline
{
    public class PrivateEndpointsCacheMiddleware : CacheMiddlewareBase
    {
        public PrivateEndpointsCacheMiddleware(RequestDelegate next,
            IConnectionMultiplexer connectionMultiplexer,
            ILogger<PrivateEndpointsCacheMiddleware> logger)
            : base(next, connectionMultiplexer, logger)
        {
        }

        public override async Task Invoke(HttpContext httpContext,
            SpecialMetadataEndpoint specialMetadataEndpoint)
        {
            if (httpContext == default)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            if (specialMetadataEndpoint == default)
            {
                throw new ArgumentNullException(nameof(specialMetadataEndpoint));
            }

            if (!specialMetadataEndpoint.IsPrivate)
            {
                await _next(httpContext);
                return;
            }

            if (!RequestIsSupported(httpContext.Request))
            {
                await _next(httpContext);
                return;
            }

            var accessToken = httpContext.Request.Headers.Authorization.ToString();
            if (accessToken == default)
            {
                await _next(httpContext);
                return;
            }

            _cacheKey = httpContext.Request.GetEncodedPathAndQuery();

            var cacheHitIsSuccess = await CacheHitAsync(httpContext.Response, accessToken);
            if (cacheHitIsSuccess)
            {
                return;
            }

            //Enabling read response body
            await BodySubstituteToReadableStream(httpContext, _next, async () =>
            {
                if (ResponseIsSupported(httpContext.Response))
                {
                    httpContext.Response.Headers.CacheControl = new StringValues(new[]
                    {
                        "no-cache", "no-store", "private"
                    });
                    await ResponseToCacheAsync(httpContext.Response, accessToken);
                }
            });
        }
    }
}