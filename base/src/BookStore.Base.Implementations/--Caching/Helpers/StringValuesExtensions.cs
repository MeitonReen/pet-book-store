using System.Text.Json;
using Microsoft.Extensions.Primitives;
using StackExchange.Redis;

namespace BookStore.Base.Implementations.__Caching.Helpers
{
    public static class StringValuesExtensions
    {
        public static RedisValue ToRedisValue(this StringValues stringValues)
        {
            return new RedisValue(JsonSerializer.Serialize(stringValues.ToArray()));
        }
    }
}