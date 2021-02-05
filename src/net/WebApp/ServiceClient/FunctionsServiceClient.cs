using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using Microsoft.AspNetCore.SignalR.Client;

public class FunctionsServiceClient : ServiceClient
{
    public FunctionsServiceClient(HttpClient httpClient) : base(httpClient)
    {
    }

    public HubConnection GetHubConnection()
    {
        return new HubConnectionBuilder()
            .WithUrl(base.httpClient.BaseAddress + "api", options =>
            {
                options.HttpMessageHandlerFactory = innerHandler =>
                new IncludeRequestCredentialsMessageHandler { InnerHandler = innerHandler };
            })
            .WithAutomaticReconnect()
            .Build();
    }
}


public class IncludeRequestCredentialsMessageHandler : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        Console.WriteLine("Hit here");
        Console.WriteLine(request.RequestUri);
        request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);
        return base.SendAsync(request, cancellationToken);
    }
}