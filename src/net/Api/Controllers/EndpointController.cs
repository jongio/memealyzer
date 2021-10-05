using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Azure.Data.AppConfiguration;
using Lib;
using Lib.Configuration;
using Lib.Model;

namespace Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EndpointController : ControllerBase
    {
        private readonly ILogger<EndpointController> logger;

        public EndpointController(ILogger<EndpointController> logger)
        {
            this.logger = logger;
        }

        [HttpGet]
        [Route("/endpoints")]
        public Dictionary<string, Endpoint> Get()
        {          
            return new Dictionary<string, Endpoint>()
            {
                {
                    "AZURE_FUNCTIONS_ENDPOINT",
                    new Endpoint()
                    {
                        Name = "AZURE_FUNCTIONS_ENDPOINT",
                        Uri = Config.IsContainerDevelopment ? "" : Config.FunctionsEndpoint.ToString()
                    }
                }
            };
        }
    }
}