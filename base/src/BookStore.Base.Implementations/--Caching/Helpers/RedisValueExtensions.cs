using System.Text.Json;
using Microsoft.Extensions.Primitives;
using StackExchange.Redis;

namespace BookStore.Base.Implementations.__Caching.Helpers
{
    public static class RedisValueExtensions
    {
        public static StringValues ToStringValues(this RedisValue redisValue)
        {
            var stringArrayInLine = redisValue.ToString();
            var stringArray = JsonSerializer.Deserialize<string[]>(stringArrayInLine);

            return new StringValues(stringArray);
        }
    }
}