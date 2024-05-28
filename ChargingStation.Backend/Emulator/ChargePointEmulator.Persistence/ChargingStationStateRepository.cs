using ChargePointEmulator.Application.Interfaces;
using ChargePointEmulator.Application.State;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace ChargePointEmulator.Persistence;

public class ChargingStationStateRepository : IChargingStationStateRepository
{
    private readonly IMongoCollection<ChargingStationStateEntity> _collection;

    public ChargingStationStateRepository(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("MongoDBConnection");
        _collection = new MongoClient(connectionString)
            .GetDatabase("charging-station-emulator")
            .GetCollection<ChargingStationStateEntity>("charging-station-states");
    }
    
    public ChargingStationStateRepository(string connectionString = "mongodb://localhost:27017")
    {
        _collection = new MongoClient(connectionString)
            .GetDatabase("charging-station-emulator")
            .GetCollection<ChargingStationStateEntity>("charging-station-states");
    }
    
    public async Task InsertAsync(ChargingStationState state, CancellationToken cancellationToken = default)
    {
        var entity = new ChargingStationStateEntity
        {
            Id = state.Id,
            JsonState = JsonConvert.SerializeObject(state)
        };
        await _collection.InsertOneAsync(entity, cancellationToken: cancellationToken);
    }
    
    public async Task<ChargingStationState?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var entity = await _collection.Find(s => s.Id == id).FirstOrDefaultAsync(cancellationToken: cancellationToken);
        var state = entity == null
            ? null
            : JsonConvert.DeserializeObject<ChargingStationState>(entity.JsonState);
        return state;
    }
    
    public async Task UpdateAsync(ChargingStationState state, CancellationToken cancellationToken = default)
    {
        var entity = new ChargingStationStateEntity
        {
            Id = state.Id,
            JsonState = JsonConvert.SerializeObject(state)
        };
        await _collection.ReplaceOneAsync(s => s.Id == state.Id, entity, cancellationToken: cancellationToken);
    }
    
    public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        await _collection.DeleteOneAsync(s => s.Id == id, cancellationToken: cancellationToken);
    }
    
    public async Task<List<ChargingStationState>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _collection.Find(s => true).ToListAsync(cancellationToken: cancellationToken);
        
        return entities.Select(entity => JsonConvert.DeserializeObject<ChargingStationState>(entity.JsonState)).ToList();
    }
}