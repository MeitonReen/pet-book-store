using BookStore.Base.Implementations.__Caching.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace BookStore.Base.Implementations.__Caching.ConfigureRequestPipeline;

public abstract class CacheMiddlewareBase
{
    protected readonly IDatabase _cacheDataBase;
    protected readonly RequestDelegate _next;

    protected string? _cacheKey = default;

    protected ILogger<CacheMiddlewareBase>? _logger;

    protected HashSet<string> _requestMethodsSupport = new() {HttpMethods.Get};
    protected HashSet<int> _responseStatusCodesSupport = new() {StatusCodes.Status200OK};

    public CacheMiddlewareBase(RequestDelegate next,
        IConnectionMultiplexer connectionMultiplexer,
        ILogger<CacheMiddlewareBase>? logger = default)
    {
        _next = next ??
                throw new ArgumentNullException(nameof(next));

        _cacheDataBase = connectionMultiplexer?.GetDatabase() ??
                         throw new ArgumentNullException(nameof(connectionMultiplexer));

        _logger = logger;
    }

    public abstract Task Invoke(HttpContext httpContext,
        SpecialMetadataEndpoint specialMetadataEndpoint);

    public async Task BodySubstituteToReadableStream(HttpContext httpContext, RequestDelegate next,
        Func<Task> doingAfterExecuteNextMiddlewaresAsync)
    {
        _logger?.LogDebug($"{nameof(BodySubstituteToReadableStream)} was started.");

        var originalResponseBodyStream = httpContext.Response.Body;

        using var responseBodyStreamSubstitute = new MemoryStream();
        httpContext.Response.Body = responseBodyStreamSubstitute;

        await next(httpContext);

        await doingAfterExecuteNextMiddlewaresAsync();

        httpContext.Response.Body = originalResponseBodyStream;
        if (httpContext.Response.Body.CanWrite && responseBodyStreamSubstitute.Length > 0)
        {
            responseBodyStreamSubstitute.Position = 0;
            await responseBodyStreamSubstitute.CopyToAsync(httpContext.Response.Body);
        }

        ;

        _logger?.LogDebug($"{nameof(BodySubstituteToReadableStream)} successfully completed...");
    }

    public async Task<bool> CacheHitAsync(HttpResponse httpResponse, string? addToKey = default)
    {
        _logger?.LogDebug($"{nameof(CacheHitAsync)} was started.");

        var responseProperties = await
            _cacheDataBase.HashGetAllAsync(_cacheKey + addToKey +
                                           CacheConstants.ResponsePropertiesPostfix);

        var responseHeaders = await
            _cacheDataBase.HashGetAllAsync(_cacheKey + addToKey +
                                           CacheConstants.ResponseHeadersPostfix);

        if (!responseProperties.Any() || !responseHeaders.Any())
        {
            _logger?.LogDebug($"Cache miss. {nameof(CacheHitAsync)} successfully completed.");
            return false;
        }

        await httpResponse.FillFromRedisHashEntriesAsync(responseProperties, responseHeaders);

        _logger?.LogDebug($"Cache hit. {nameof(CacheHitAsync)} successfully completed.");
        return true;
    }

    public async Task<bool> ResponseToCacheAsync(HttpResponse httpResponse,
        string? addToKey = default)
    {
        _logger?.LogDebug($"{nameof(ResponseToCacheAsync)} was started.");

        var (hashEntriesPropertiesWithoutHeaders, hashEntriesHeaders) =
            await httpResponse.ToRedisHashEntriesAsync();

        try
        {
            await _cacheDataBase.HashSetAsync(_cacheKey + addToKey +
                                              CacheConstants.ResponsePropertiesPostfix,
                hashEntriesPropertiesWithoutHeaders);

            await _cacheDataBase.HashSetAsync(_cacheKey + addToKey +
                                              CacheConstants.ResponseHeadersPostfix,
                hashEntriesHeaders);

            _logger?.LogDebug($"Response is cached. {nameof(ResponseToCacheAsync)} successfully completed.");
            return true;
        }
        catch
        {
            _logger?.LogDebug($"Response caching failed. {nameof(ResponseToCacheAsync)} successfully completed.");
            return false;
        }
    }

    public bool RequestIsSupported(HttpRequest request) =>
        _requestMethodsSupport.Contains(request.Method);

    public bool ResponseIsSupported(HttpResponse response) =>
        _responseStatusCodesSupport.Contains(response.StatusCode);
}