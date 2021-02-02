using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lib;
using Lib.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ImageController : ControllerBase
    {
        private readonly ILogger<ImageController> logger;
        private Clients clients;

        public ImageController(ILogger<ImageController> logger, Clients clients)
        {
            this.logger = logger;
            this.clients = clients;

        }

        [HttpPost]
        public async Task<Image> Post([FromBody] Image image = null)
        {
            return await clients.EnqueueImageAsync(image);
        }

        [HttpGet]
        [Route("/images")]
        public async IAsyncEnumerable<Image> Get()
        {
            await foreach (var image in clients.DataProvider.GetImagesAsync())
            {
                yield return image;
            }
        }

        [HttpGet]
        [Route("/image/{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var image = await clients.DataProvider.GetImageAsync(id);
            if(image is null){
                return NotFound(id);
            }
            return Ok(image);
        }

        [HttpDelete]
        [Route("/image/{id}")]
        public async Task<IActionResult> DeleteById(string id)
        {
            var image = await clients.DataProvider.DeleteImageAsync(id);
            return Ok(image);
        }
    }
}