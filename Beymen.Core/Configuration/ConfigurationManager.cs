using Beymen.Core.Configuration.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Beymen.Core.Configuration
{
    public class ConfigurationManager : IConfigurationManager
    {
        private IMongoCollection<ConfigurationItem> _configCollection;

        public ConfigurationManager(string mongoConnectionString)
        {
            var mongoClient = new MongoClient(mongoConnectionString);

            var configdatabase = mongoClient.GetDatabase("configdatabase");

            _configCollection = configdatabase.GetCollection<ConfigurationItem>("configcollection");
        }

        public Task<List<ConfigurationItem>> GetAllAsync()
        {
            return _configCollection.Find(Builders<ConfigurationItem>.Filter.Empty).ToListAsync();
        }

        public Task<List<ConfigurationItem>> GetServiceValuesAsync(string applicationName)
        {
            var filter = Builders<ConfigurationItem>.Filter.Eq(n => n.ApplicationName, applicationName)
             & Builders<ConfigurationItem>.Filter.Eq(n => n.IsActive, true);

            return _configCollection.Find(filter).ToListAsync();
        }

        public async Task<string> GetValueAsync(string applicationName, string name)
        {
            var filter = Builders<ConfigurationItem>.Filter.Eq(n => n.ApplicationName, applicationName)
             & Builders<ConfigurationItem>.Filter.Eq(n => n.Name, name)
             & Builders<ConfigurationItem>.Filter.Eq(n => n.IsActive, true);

            var result = await _configCollection.Find(filter).FirstOrDefaultAsync();

            return result?.Value;
        }

        public Task InsertOneAsync(ConfigurationItem item)
        {
            return _configCollection.InsertOneAsync(item);
        }

        public Task DeleteManyAsync(string applicationName)
        {
            return _configCollection.DeleteManyAsync(x => x.ApplicationName == applicationName);
        }

        public Task<ConfigurationItem> UpdateAsync(string applicationName, string name, string value)
        {
            var filterResult = Builders<ConfigurationItem>.Filter.Eq(n => n.ApplicationName, applicationName)
                & Builders<ConfigurationItem>.Filter.Eq(n => n.Name, name);

            var updateStatement = Builders<ConfigurationItem>.Update.Set(x => x.Value, value);

            return _configCollection.FindOneAndUpdateAsync(filterResult, updateStatement);
        }
    }
}
