using System;
using Azure;
using Azure.Data.Tables;

namespace Lib.Model
{
    public class TableImage : Image, ITableEntity
    {
        public TableImage()
        {
        }

        public ETag ETag { get; set; }
        public string RowKey { get => base.Id; set => base.Id = value; }
        public DateTimeOffset? Timestamp { get; set; }
    }
}