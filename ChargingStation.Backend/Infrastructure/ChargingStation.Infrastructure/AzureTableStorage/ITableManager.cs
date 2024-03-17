namespace ChargingStation.Infrastructure.AzureTableStorage
{
    public interface ITableManager<T> where T : BaseTableStorageEntity
    {
        Task AddEntityAsync(string tableName, T entity);
        Task<T> GetEntityAsync(string tableName, string partitionKey, string rowKey);
        Task<List<T>> GetEntitiesByPartitionKeyAsync(string tableName, string partitionKey);
        Task<List<T>> GetAllEntitiesAsync(string tableName);
        Task UpsertEntityAsync(string tableName, T entity);
        Task DeleteEntityAsync(string tableName, T entity);
    }
}
