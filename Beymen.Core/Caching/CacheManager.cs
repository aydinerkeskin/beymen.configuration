using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace Beymen.Core.Caching
{
    public class CacheManager : ICacheManager
    {
        private IDatabase _database;

        public CacheManager(string redisConnectionString)
        {
            var redisClient = ConnectionMultiplexer.Connect(ConfigurationOptions.Parse(redisConnectionString));
            _database = redisClient.GetDatabase();
        }

        public async Task<string> GetAsync(string key)
        {
            string value = await _database.StringGetAsync(key);

            return value;
        }

        public async Task<string> UpdateAsync(string key, string value, TimeSpan? expiry = null)
        {
            await _database.StringSetAsync(key, value, expiry);

            return value;
        }

        public Task DeleteAsync(string key)
        {
            return _database.KeyDeleteAsync(key);
        }
    }
}
