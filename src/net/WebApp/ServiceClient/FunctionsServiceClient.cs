using System.Net.Http;
using Microsoft.AspNetCore.SignalR.Client;

public class FunctionsServiceClient : ServiceClient
{
    public FunctionsServiceClient(HttpClient httpClient) : base(httpClient)
    {
    }

    public HubConnection GetHubConnection()
    {
        return new HubConnectionBuilder()
            .WithUrl(base.httpClient.BaseAddress + "api")
            .WithAutomaticReconnect()
            .Build();
    }
}