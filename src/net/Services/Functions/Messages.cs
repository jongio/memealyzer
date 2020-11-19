using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Lib.Model;

namespace Memealyzer.Functions
{
    public static class Messages
    {
        public static async Task SendAsync(IAsyncCollector<SignalRMessage> messages, string message)
        {
            var image = JsonSerializer.Deserialize<Image>(message, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            await messages.AddAsync(new SignalRMessage { Target = "ReceiveImage", Arguments = new[] { image } });
        }
    }
}