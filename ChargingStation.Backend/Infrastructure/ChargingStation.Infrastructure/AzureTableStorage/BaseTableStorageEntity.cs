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

        public BaseTableStorageEntity(string partitionKey, ETag eTag)
        {
            PartitionKey = partitionKey;
            Timestamp = DateTimeOffset.UtcNow;
            RowKey = GenerateRowKey(Timestamp.Value);
            ETag = eTag;
        }

        public BaseTableStorageEntity(string partitionKey)
        {
            PartitionKey = partitionKey;
            Timestamp = DateTimeOffset.UtcNow;
            RowKey = GenerateRowKey(Timestamp.Value);
        }

        private string GenerateRowKey(DateTimeOffset timestamp)
        {
            long ticksDifference = DateTime.MaxValue.Ticks - timestamp.Ticks;

            string rowKey = ticksDifference.ToString("d19");

            return rowKey;
        }
    }
}