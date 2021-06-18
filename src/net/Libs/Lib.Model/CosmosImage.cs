using System;
using Azure;
using Azure.Cosmos;
using Azure.Data.Tables;

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