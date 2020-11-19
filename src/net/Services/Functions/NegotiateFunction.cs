using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;

namespace Memealyzer.Functions
{
    public static class NegotiateFunction
    {
        [FunctionName("negotiate")]
        public static SignalRConnectionInfo Negotiate(
                    [HttpTrigger(AuthorizationLevel.Anonymous)] HttpRequest req,
                    [SignalRConnectionInfo(HubName = "%MessagingType%")] SignalRConnectionInfo connectionInfo)
        {
            return connectionInfo;
        }
    }
}