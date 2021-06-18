using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Azure.Data.AppConfiguration;
using Lib;

namespace Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ConfigController : ControllerBase
    {
        private readonly ILogger<ConfigController> logger;
        private Clients clients;

        public ConfigController(ILogger<ConfigController> logger, Clients clients)
        {
            this.logger = logger;
            this.clients = clients;
        }

        [HttpGet]
        [Route("/config")]
        public async Task<ConfigurationSetting> Get([FromQuery] string name)
        {
            //TODO: Move try/catch to here?
            return await clients.ConfigurationClient.GetConfigurationSettingAsync(name);
        }
    }
}