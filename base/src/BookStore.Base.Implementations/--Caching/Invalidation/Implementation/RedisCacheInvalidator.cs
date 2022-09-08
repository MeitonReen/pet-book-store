using BookStore.Base.Implementations.__Caching.Helpers;
using BookStore.Base.Implementations.__Caching.Invalidation.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace BookStore.Base.Implementations.__Caching.Invalidation.Implementation
{
    public class RedisCacheInvalidator : ICacheInvalidator
    {
        private readonly IDatabase _cacheDataBase;
        private readonly ILogger<RedisCacheInvalidator>? _logger;
        private readonly IAuthorizationPolicyProvider _policyProvider;

        public RedisCacheInvalidator(IConnectionMultiplexer connectionMultiplexer,
            IAuthorizationPolicyProvider policyProvider, ILogger<RedisCacheInvalidator>? logger
                = default)
        {
            _cacheDataBase = connectionMultiplexer?.GetDatabase() ??
                             throw new ArgumentNullException(nameof(connectionMultiplexer));

            _policyProvider = policyProvider ??
                              throw new ArgumentNullException(nameof(policyProvider));

            _logger = logger;
        }

        public async Task<bool> InvalidateAsync(ControllerBase targetController,
            Type targetControllerType, string readResourceMethodName, object? readResourceRequest)
        {
            _logger?.LogDebug($"{nameof(InvalidateAsync)} was started.");

            var cacheKey = targetController.Url.Action(readResourceMethodName, readResourceRequest);

            if (await TargetMethodIsPrivateAsync(targetControllerType, readResourceMethodName))
            {
                var accessToken = targetController.HttpContext.Request.Headers.Authorization
                    .ToString();

                cacheKey += accessToken;
            }

            var RemovedCount = await _cacheDataBase.KeyDeleteAsync(new RedisKey[]
            {
                cacheKey + CacheConstants.ResponseHeadersPostfix,
                cacheKey + CacheConstants.ResponseHeadersPostfix,
            });

            if (RemovedCount > 0)
            {
                _logger?.LogDebug($"{nameof(InvalidateAsync)} cache is invalidate.");
                return true;
            }

            _logger?.LogDebug($"{nameof(InvalidateAsync)} there is not cache for this key.");
            return false;
        }

        private async Task<bool> TargetMethodIsPrivateAsync(Type targetControllerType,
            string readResourceMethodName)
        {
            var targetMethodAttributes = targetControllerType.GetMethod(readResourceMethodName)
                ?.CustomAttributes;

            var targetMethodIsAllowedAnonymous =
                targetMethodAttributes!.Any(attribute => attribute
                    .GetType() == typeof(IAllowAnonymous));

            if (targetMethodIsAllowedAnonymous)
            {
                return false;
            }

            var authorizeData = targetMethodAttributes
                ?.Where(attribute => attribute.GetType() == typeof(IAuthorizeData))
                .Select(attribute => (IAuthorizeData) attribute);

            authorizeData ??= Enumerable.Empty<IAuthorizeData>();

            var policy = await AuthorizationPolicy.CombineAsync(_policyProvider, authorizeData);

            if (policy == default)
            {
                return false;
            }

            return true;
        }
    }
}