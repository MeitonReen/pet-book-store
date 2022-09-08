using BookStore.Base.Implementations.__Caching.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace BookStore.Base.Implementations.__Caching.ConfigureRequestPipeline;

public class PublicEndpointsCacheMiddleware : CacheMiddlewareBase
{
    private readonly IAuthorizationPolicyProvider _policyProvider;

    public PublicEndpointsCacheMiddleware(RequestDelegate next,
        IAuthorizationPolicyProvider policyProvider,
        IConnectionMultiplexer connectionMultiplexer,
        ILogger<PublicEndpointsCacheMiddleware> logger)
        : base(next, connectionMultiplexer, logger)
    {
        _policyProvider = policyProvider ??
                          throw new ArgumentNullException(nameof(policyProvider));
    }

    public override async Task Invoke(HttpContext httpContext,
        SpecialMetadataEndpoint specialMetadataEndpoint)
    {
        _logger?.LogDebug($"{nameof(Invoke)} was started.");

        if (httpContext == default)
        {
            throw new ArgumentNullException(nameof(httpContext));
        }

        if (specialMetadataEndpoint == default)
        {
            throw new ArgumentNullException(nameof(specialMetadataEndpoint));
        }

        if (!RequestIsSupported(httpContext.Request))
        {
            await _next(httpContext);
            return;
        }

        var endpoint = httpContext.GetEndpoint();
        if (endpoint == default)
        {
            await _next(httpContext);
            return;
        }

        if (endpoint.Metadata.GetMetadata<CacheIgnoreAttribute>() != default)
        {
            await _next(httpContext);
            return;
        }

        _cacheKey = httpContext.Request.GetEncodedPathAndQuery();

        var authorizationPolicy = await GetAuthorizationPolicyAsync(endpoint);


        if (authorizationPolicy == default) //Policy is not specified
        {
            specialMetadataEndpoint.IsPrivate = false;

            var cacheHitIsSuccess = await CacheHitAsync(httpContext.Response);
            if (cacheHitIsSuccess)
            {
                return;
            }

            //Enabling read response body
            await BodySubstituteToReadableStream(httpContext, _next, async () =>
            {
                if (ResponseIsSupported(httpContext.Response))
                {
                    await ResponseToCacheAsync(httpContext.Response);
                }
            });
            return;
        }

        if (endpoint?.Metadata.GetMetadata<IAllowAnonymous>() != default) //AllowAnonymous specified
        {
            specialMetadataEndpoint.IsPrivate = false;

            var cacheHitIsSuccess = await CacheHitAsync(httpContext.Response);
            if (cacheHitIsSuccess)
            {
                return;
            }

            //Enabling read response body
            await BodySubstituteToReadableStream(httpContext, _next, async () =>
            {
                if (ResponseIsSupported(httpContext.Response))
                {
                    await ResponseToCacheAsync(httpContext.Response);
                }
            });

            return;
        }

        specialMetadataEndpoint.IsPrivate = true;
        await _next(httpContext);
    }

    private async Task<AuthorizationPolicy?> GetAuthorizationPolicyAsync(Endpoint endpoint)
    {
        var authorizeData = endpoint?.Metadata.GetOrderedMetadata<
            IAuthorizeData>() ?? Array.Empty<IAuthorizeData>();
        return await AuthorizationPolicy.CombineAsync(_policyProvider,
            authorizeData);
    }
}