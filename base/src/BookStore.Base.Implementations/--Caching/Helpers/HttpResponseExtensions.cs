using Microsoft.AspNetCore.Http;
using StackExchange.Redis;

namespace BookStore.Base.Implementations.__Caching.Helpers
{
    public static class HttpResponseExtensions
    {
        public static async Task FillFromRedisHashEntriesAsync(this HttpResponse httpResponse,
            HashEntry[] propertiesWithoutHeaders,
            HashEntry[] headers)
        {
            httpResponse.Headers.FillFromRedisHashEntries(headers);

            httpResponse.StatusCode = (int) propertiesWithoutHeaders.GetValue(
                nameof(httpResponse.StatusCode));
            httpResponse.ContentType = propertiesWithoutHeaders.GetValue(
                nameof(httpResponse.ContentType));

            byte[] bodyFromCache = propertiesWithoutHeaders.GetValue(
                nameof(httpResponse.Body));

            await httpResponse.Body.WriteAsync(bodyFromCache);
        }

        public static async Task<(HashEntry[] propertiesWithoutHeaders,
                HashEntry[] headers)>
            ToRedisHashEntriesAsync(this HttpResponse httpResponse)
        {
            var headers = httpResponse.Headers.ToRedisHashEntries();

            var bodyOriginalPosition = httpResponse.Body.Position;
            httpResponse.Body.Position = 0;
            var propertiesWithoutHeaders = new[]
            {
                new HashEntry(nameof(httpResponse.StatusCode),
                    httpResponse.StatusCode),
                new HashEntry(nameof(httpResponse.Body),
                    await new StreamReader(httpResponse.Body).ReadToEndAsync()),
                new HashEntry(nameof(httpResponse.ContentType),
                    httpResponse.ContentType)
            };
            httpResponse.Body.Position = bodyOriginalPosition;

            return (propertiesWithoutHeaders, headers);
        }
    }
}