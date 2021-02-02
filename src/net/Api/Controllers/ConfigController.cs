using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Data.AppConfiguration;
using Lib;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

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