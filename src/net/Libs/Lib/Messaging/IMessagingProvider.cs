using System;
using System.Threading.Tasks;
using Azure.Core;
using Lib.Data;

namespace Lib.Messaging
{
    public interface IMessagingProvider : IAsyncDisposable
    {
        Task InitializeAsync(TokenCredential credential, IDataProvider dataProvider);
        IQueue ImageQueueClient { get; set; }
        IQueue ClientSyncQueueClient { get; set; }
    }
}