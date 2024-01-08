using Beymen.Core.Caching;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace Beymen.Core.Configuration
{
    public class ConfigurationReader
    {
        private string _applicationName = string.Empty;
        private int _refreshTimerIntervalInMs = 5000;

        private ICacheManager _cacheManager = null;
        private IConfigurationManager _configurationManager = null;

        private System.Timers.Timer _timer;

        public ConfigurationReader(string applicationName, string connectionString, int refreshTimerIntervalInMs, string redisConnectionString)
        {
            _applicationName = applicationName;
            _refreshTimerIntervalInMs = refreshTimerIntervalInMs;

            _cacheManager = new CacheManager(redisConnectionString);
            _configurationManager = new ConfigurationManager(connectionString);

            _timer = new System.Timers.Timer(_refreshTimerIntervalInMs);
            _timer.Elapsed += ConfigurationTimerHandler;
            _timer.AutoReset = true;
            _timer.Enabled = true;
        }

        public async Task<T> GetValue<T>(string key)
        {
            var value = await _cacheManager.GetAsync(GetCacheKey(_applicationName, key));

            if (value == null)
            {
                value = await _configurationManager.GetValueAsync(_applicationName, key);

                if (!string.IsNullOrEmpty(value))
                {
                    await _cacheManager.UpdateAsync(GetCacheKey(_applicationName, key), value);
                }
            }

            if (value != null)
            {
                return (T)Convert.ChangeType(value, typeof(T));
            }

            return default(T);
        }

        private async void ConfigurationTimerHandler(Object source, ElapsedEventArgs e)
        {
            Console.WriteLine($"ApplicationName: [{_applicationName}] timer elapsed.");

            try
            {
                var items = await _configurationManager.GetServiceValuesAsync(_applicationName);

                foreach (var item in items)
                {
                    var cacheItemValue = await _cacheManager.GetAsync(GetCacheKey(_applicationName, item.Name));

                    if (!item.Value.Equals(cacheItemValue))
                    {
                        await _cacheManager.UpdateAsync(GetCacheKey(_applicationName, item.Name), item.Value);

                        Console.WriteLine($"ApplicationName: [{_applicationName}], Name: [{item.Name}] cache updated.");
                    }
                    else
                    {
                        Console.WriteLine($"ApplicationName: [{_applicationName}], Name: [{item.Name}] cache equal.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ApplicationName: [{_applicationName}] timer ex. message: [{ex.Message}]");
            }

            
        }

        private string GetCacheKey(string applicationName, string key)
        {
            return $"{applicationName}_{key}";
        }
    }
}
