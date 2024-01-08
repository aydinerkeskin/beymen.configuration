using Beymen.Core.Configuration;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Beymen.Configuration.ReadAPI.Controllers
{
    [ApiController]
    [Route("api/read")]
    public class ReadController : ControllerBase
    {
        private readonly ConfigurationReader _configurationReader;

        public ReadController(ConfigurationReader configurationReader)
        {
            _configurationReader = configurationReader;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            var serviceConfigurationItems = new List<string>();

            serviceConfigurationItems.Add($"[SiteName] => {await _configurationReader.GetValue<string>("SiteName")}");
            serviceConfigurationItems.Add($"[IsBasketEnabled] => {await _configurationReader.GetValue<bool>("IsBasketEnabled")}");
            serviceConfigurationItems.Add($"[MaxItemCount] => {await _configurationReader.GetValue<int>("MaxItemCount")}");

            return Ok(serviceConfigurationItems);
        }
    }
}
