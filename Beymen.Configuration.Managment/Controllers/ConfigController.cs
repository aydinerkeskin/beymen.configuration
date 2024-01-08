using Beymen.Configuration.Managment.Models;
using Beymen.Core.Configuration;
using Beymen.Core.Configuration.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Beymen.Configuration.Managment.Controllers
{
    [ApiController]
    [Route("api/config")]
    public class ConfigController : ControllerBase
    {
        private readonly IConfigurationManager _configurationManager;

        public ConfigController(IConfigurationManager configurationManager)
        {
            _configurationManager = configurationManager;
        }

        [HttpGet]
        public async Task<GetConfigurationItemsResult> GetConfigurationItemsAsync()
        {
            var configurationItems = await _configurationManager.GetAllAsync();

            return new GetConfigurationItemsResult { Items = configurationItems };
        }

        [HttpPost]
        public async Task<IActionResult> CreateConfigurationItemAsync([FromBody] ConfigurationItem request)
        {
            var value = await _configurationManager.GetValueAsync(request.ApplicationName, request.Name);

            if (value != null)
            {
                throw new System.Exception("ApplicationName and Name already exists!");
            }

            await _configurationManager.InsertOneAsync(request);

            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateConfigurationItemAsync(string id, [FromBody] ConfigurationItem request)
        {
            var value = await _configurationManager.GetValueAsync(request.ApplicationName, request.Name);

            if (value != null)
            {
                await _configurationManager.UpdateAsync(request.ApplicationName, request.Name, request.Value);
            }

            return Ok();
        }
    }
}
