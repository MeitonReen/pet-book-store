using Microsoft.AspNetCore.Http;
using StackExchange.Redis;

namespace BookStore.Base.Implementations.__Caching.Helpers
{
    public static class IHeaderDictionaryExtensions
    {
        public static HashEntry[] ToRedisHashEntries(
            this IHeaderDictionary headerDictionary)
        {
            var propertiesWithoutHeaders = new List<HashEntry>();

            var headers = headerDictionary.Select(header =>
                new HashEntry(header.Key, header.Value.ToRedisValue()));

            propertiesWithoutHeaders.AddRange(headers);

            return propertiesWithoutHeaders.ToArray();
        }

        public static void FillFromRedisHashEntries(
            this IHeaderDictionary headerDictionary, HashEntry[] responseHeaders)
        {
            Array.ForEach(responseHeaders,
                header => { headerDictionary.Add(header.Name, header.Value.ToStringValues()); });
        }
    }
}