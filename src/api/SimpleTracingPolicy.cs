using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Azure.Core;
using Azure.Core.Pipeline;


public class SimpleTracingPolicy : HttpPipelinePolicy
{
    ILogger<SimpleTracingPolicy> _logger;

    public SimpleTracingPolicy(ILogger<SimpleTracingPolicy> logger)
    {
        _logger = logger;
    }

    public override async ValueTask ProcessAsync(HttpMessage message, ReadOnlyMemory<HttpPipelinePolicy> pipeline)
    {
        _logger.LogInformation($">> Request: {message.Request.Method} {message.Request.Uri}");
        
        // As an example, you can add custom headers via policies
        message.Request.Headers.Add("x-custom-header", DateTime.Now.Ticks.ToString());
        await ProcessNextAsync(message, pipeline);
        _logger.LogInformation($">> Response: {message.Response.Status} from {message.Request.Method} {message.Request.Uri}\n");
    }

    #region public override void Process
    public override void Process(HttpMessage message, ReadOnlyMemory<HttpPipelinePolicy> pipeline)
    {
        _logger.LogInformation($">> Request: {message.Request.Uri}");
        ProcessNext(message, pipeline);
        _logger.LogInformation($">> Response: {message.Response.Status} {message.Request.Method} {message.Request.Uri}");
    }
    #endregion public override void Process
}