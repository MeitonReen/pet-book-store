using StackExchange.Redis;

namespace BookStore.Base.Implementations.__Caching.Helpers
{
    public static class RedisHashEntryExtensions
    {
        public static RedisValue GetValue(this HashEntry[] hashEntries, string name)
        {
            return hashEntries.First(prop => prop.Name == name).Value;
        }
    }
}