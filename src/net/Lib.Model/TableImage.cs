using System;
using Azure.Data.Tables;
using Azure;

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