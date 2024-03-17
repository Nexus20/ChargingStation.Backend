using Azure.Data.Tables;

namespace ChargingStation.Infrastructure.AzureTableStorage
{
    public class AzureTableStorageManager<T> : ITableManager<T> where T : BaseTableStorageEntity
    {
        private readonly TableServiceClient _tableServiceClient;

        public AzureTableStorageManager(TableServiceClient tableServiceClient)
        {
            _tableServiceClient = tableServiceClient;
        }

        public async Task AddEntityAsync(string tableName, T entity)
        {
            var tableClient = _tableServiceClient.GetTableClient(tableName);
            await tableClient.AddEntityAsync(entity);
        }

        public async Task<T> GetEntityAsync(string tableName, string partitionKey, string rowKey)
        {
            var tableClient = _tableServiceClient.GetTableClient(tableName);
            return await tableClient.GetEntityAsync<T>(partitionKey, rowKey);
        }

        public async Task<List<T>> GetEntitiesByPartitionKeyAsync(string tableName, string partitionKey)
        {
            var tableClient = _tableServiceClient.GetTableClient(tableName);
            return await tableClient.QueryAsync<T>(filter: $"PartitionKey eq '{partitionKey}'").ToListAsync();
        }

        public async Task<List<T>> GetAllEntitiesAsync(string tableName)
        {
            var tableClient = _tableServiceClient.GetTableClient(tableName);
            return await tableClient.QueryAsync<T>().ToListAsync();
        }

        public async Task UpsertEntityAsync(string tableName, T entity)
        {
            var tableClient = _tableServiceClient.GetTableClient(tableName);
            await tableClient.UpsertEntityAsync(entity);
        }

        public async Task DeleteEntityAsync(string tableName, T entity)
        {
            var tableClient = _tableServiceClient.GetTableClient(tableName);
            await tableClient.DeleteEntityAsync(entity.PartitionKey, entity.RowKey);
        }
    }

}
