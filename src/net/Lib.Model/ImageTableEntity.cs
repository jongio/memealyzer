using System;
using Azure.Data.Tables;
using Azure;

namespace Lib.Model
{
    public class ImageTableEntity : Image, ITableEntity
    {
        private string partitionKey;
        private string rowKey;
        private DateTimeOffset? timestamp;
        private ETag eTag;

        public ImageTableEntity()
        {
        }

        public string PartitionKey
        {
            get
            {
                if (string.IsNullOrEmpty(partitionKey))
                {
                    partitionKey = this.Id.Substring(0, 4);
                }
                return partitionKey;
            }
            set => partitionKey = value;
        }
        public string RowKey
        {
            get
            {
                if (string.IsNullOrEmpty(rowKey))
                {
                    rowKey = this.Id;
                }
                return rowKey;
            }
            set => rowKey = value;
        }
        public DateTimeOffset? Timestamp { get => timestamp; set => timestamp = value; }
        public ETag ETag { get => eTag; set => eTag = value; }
    }
}