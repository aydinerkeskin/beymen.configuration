using Beymen.Core.Caching;
using Beymen.Core.Configuration;
using Beymen.Core.Configuration.Models;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Beymen.Test
{
    public class MainTests
    {
        private const string MONGO_CONNECTION_STRING = "mongodb://localhost:27017";
        private const string REDIS_CONNECTION_STRING = "127.0.0.1:6379,abortconnect=false";
        private const int REFRESH_TIMER_INTERVAL_IN_MS = 50000;

        ConfigurationManager _configurationManager;
        CacheManager _cacheManager;

        public MainTests()
        {
            _configurationManager = new ConfigurationManager(MONGO_CONNECTION_STRING);
            _cacheManager = new CacheManager(REDIS_CONNECTION_STRING);
        }

        [Fact]
        public async Task Test_GetValue_Successfully()
        {
            var configurationReader = new ConfigurationReader("SERVICE-A", MONGO_CONNECTION_STRING, REFRESH_TIMER_INTERVAL_IN_MS, REDIS_CONNECTION_STRING);

            await InsertSampleConfigurationItemsAsync();

            string siteName = await configurationReader.GetValue<string>("SiteName");
            int maxItemCount = await configurationReader.GetValue<int>("MaxItemCount");
            bool isBasketEnabled = await configurationReader.GetValue<bool>("IsBasketEnabled");

            await DeleteSampeConfigurationItemsAsync();

            Assert.Equal("beymen.com.tr", siteName);
            Assert.True(isBasketEnabled);
            Assert.Equal(5, maxItemCount);
        }

        private async Task InsertSampleConfigurationItemsAsync()
        {
            await _configurationManager.InsertOneAsync(new ConfigurationItem
            {
                ApplicationName = "SERVICE-T",
                Name = "SiteName",
                Type = "String",
                Value = "beymen.com.tr",
                IsActive = true
            });

            await _configurationManager.InsertOneAsync(new ConfigurationItem
            {
                ApplicationName = "SERVICE-T",
                Name = "IsBasketEnabled",
                Type = "Boolean",
                Value = "true",
                IsActive = true
            });

            await _configurationManager.InsertOneAsync(new ConfigurationItem
            {
                ApplicationName = "SERVICE-T",
                Name = "MaxItemCount",
                Type = "Int",
                Value = "5",
                IsActive = true
            });
        }

        private async Task DeleteSampeConfigurationItemsAsync()
        {
            await _cacheManager.DeleteAsync(GetCacheKey("SERVICE-T", "SiteName"));
            await _cacheManager.DeleteAsync(GetCacheKey("SERVICE-T", "IsBasketEnabled"));
            await _cacheManager.DeleteAsync(GetCacheKey("SERVICE-T", "MaxItemCount"));

            await _configurationManager.DeleteManyAsync("SERVICE-T");
        }

        private string GetCacheKey(string applicationName, string key)
        {
            return $"{applicationName}_{key}";
        }
    }
}
