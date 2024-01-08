using Beymen.Core.Configuration.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Beymen.Core.Configuration
{
    public interface IConfigurationManager
    {
        Task<List<ConfigurationItem>> GetAllAsync();

        Task<List<ConfigurationItem>> GetServiceValuesAsync(string applicationName);

        Task<string> GetValueAsync(string applicationName, string name);

        Task InsertOneAsync(ConfigurationItem item);

        Task DeleteManyAsync(string applicationName);

        Task<ConfigurationItem> UpdateAsync(string applicationName, string name, string value);
    }
}
