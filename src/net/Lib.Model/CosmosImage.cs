using System;
using Azure.Data.Tables;
using Azure;
using Azure.Cosmos;

namespace Lib.Model
{
    public class CosmosImage : Image
    {
        public CosmosImage()
        {
        }

        public PartitionKey PartitionKeyValue { get => new PartitionKey(this.PartitionKey); }
    }
}