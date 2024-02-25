using Azure.Data.Tables;

namespace ChargingStation.Infrastructure.AzureTableStorage
{
    public class AzureTableStorageManager<T> where T : BaseTableStorageEntity
    {
        private readonly TableServiceClient _tableServiceClient;
        private readonly string _tableName;

        public AzureTableStorageManager(string connectionString, string tableName)
        {
            _tableServiceClient = new TableServiceClient(connectionString);
            _tableName = tableName;
        }

        public async Task AddEntityAsync(T entity)
        {
            var tableClient = GetTableClient();
            await tableClient.AddEntityAsync(entity);
        }

        public async Task<T> GetEntityAsync(string partitionKey, string rowKey)
        {
            var tableClient = GetTableClient();
            return await tableClient.GetEntityAsync<T>(partitionKey, rowKey);
        }

        public async Task<IEnumerable<T>> GetEntitiesByPartitionKeyAsync(string partitionKey)
        {
            var tableClient = GetTableClient();
            return await tableClient.QueryAsync<T>(filter: $"PartitionKey eq '{partitionKey}'").ToListAsync();
        }

        public async Task<IEnumerable<T>> GetAllEntitiesAsync()
        {
            var tableClient = GetTableClient();
            return await tableClient.QueryAsync<T>().ToListAsync();
        }

        public async Task UpsertEntityAsync(T entity)
        {
            var tableClient = GetTableClient();
            await tableClient.UpsertEntityAsync(entity);
        }

        public async Task DeleteEntityAsync(T entity)
        {
            var tableClient = GetTableClient();
            await tableClient.DeleteEntityAsync(entity.PartitionKey, entity.RowKey);
        }


        private TableClient GetTableClient()
        {
            return _tableServiceClient.GetTableClient(_tableName);
        }
    }
}
