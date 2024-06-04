using Azure.Data.Tables;
using ChargingStation.TableStorage.Models;

namespace ChargingStation.TableStorage.Managers;

public class AzureTableStorageManager<T> : ITableManager<T> where T : BaseTableStorageEntity
{
    private readonly TableServiceClient _tableServiceClient;

    public AzureTableStorageManager(TableServiceClient tableServiceClient)
    {
        _tableServiceClient = tableServiceClient;
    }

    public async Task AddEntityAsync(string tableName, T entity)
    {
        var tableClient = await GetTableClientAsync(tableName);
        await tableClient.AddEntityAsync(entity);
    }

    public async Task<T> GetEntityAsync(string tableName, string partitionKey, string rowKey)
    {
        var tableClient = await GetTableClientAsync(tableName);
        return await tableClient.GetEntityAsync<T>(partitionKey, rowKey);
    }

    public async Task<List<T>> GetEntitiesByPartitionKeyAsync(string tableName, string partitionKey)
    {
        var tableClient = await GetTableClientAsync(tableName);
        return await tableClient.QueryAsync<T>(filter: $"PartitionKey eq '{partitionKey}'").ToListAsync();
    }

    public async Task<List<T>> GetAllEntitiesAsync(string tableName)
    {
        var tableClient = await GetTableClientAsync(tableName);
        return await tableClient.QueryAsync<T>().ToListAsync();
    }

    public async Task UpsertEntityAsync(string tableName, T entity)
    {
        var tableClient = await GetTableClientAsync(tableName);
        await tableClient.UpsertEntityAsync(entity);
    }

    public async Task DeleteEntityAsync(string tableName, T entity)
    {
        var tableClient = await GetTableClientAsync(tableName);
        await tableClient.DeleteEntityAsync(entity.PartitionKey, entity.RowKey);
    }
    
    private async Task<TableClient> GetTableClientAsync(string tableName)
    {
        await _tableServiceClient.CreateTableIfNotExistsAsync(tableName);
        var tableClient = _tableServiceClient.GetTableClient(tableName);
        return tableClient;
    }
}