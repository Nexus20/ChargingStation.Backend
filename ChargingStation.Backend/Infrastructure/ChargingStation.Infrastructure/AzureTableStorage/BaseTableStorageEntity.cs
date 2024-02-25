using Azure;
using Azure.Data.Tables;

namespace ChargingStation.Infrastructure.AzureTableStorage
{
    public class BaseTableStorageEntity : ITableEntity
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }

        BaseTableStorageEntity(string partitionKey, string rowKey, DateTimeOffset? timestamp, ETag eTag)
        {
            PartitionKey = partitionKey;
            RowKey = rowKey;
            Timestamp = timestamp;
            ETag = eTag;
        }
    }
}
