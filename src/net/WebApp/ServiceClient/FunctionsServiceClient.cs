using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

public class FunctionsServiceClient : ServiceClient
{
    private ApiServiceClient apiServiceClient;

    public FunctionsServiceClient(HttpClient httpClient, ApiServiceClient apiServiceClient) : base(httpClient)
    {
        this.apiServiceClient = apiServiceClient;
    }

    public async Task InitializeAsync()
    {
        var endpoints = await this.apiServiceClient.GetEndpoints();
        Endpoint azureFunctionsEndpoint;
        if(endpoints.TryGetValue("AZURE_FUNCTIONS_ENDPOINT", out azureFunctionsEndpoint))
        {
            if(!string.IsNullOrEmpty(azureFunctionsEndpoint.Uri))
            {
                Console.WriteLine(azureFunctionsEndpoint.Uri);
                base.httpClient.BaseAddress = new Uri(azureFunctionsEndpoint.Uri);
            }
        }
        
    }

    public HubConnection GetHubConnection()
    {
        return new HubConnectionBuilder()
            .WithUrl(base.httpClient.BaseAddress + "api")
            .WithAutomaticReconnect()
            .Build();
    }
}