using System.Collections.Concurrent;
using ChargePointEmulator.Application.Interfaces;
using ChargePointEmulator.Application.State;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ChargePointEmulator.Application.Services;

public class ChargingStationSimulatorManager
{
    public ConcurrentDictionary<Guid, ChargingStation> ChargingStations { get; } = new();

    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly string _centralSystemEndpoint;
    private readonly string _hubEndpoint;

    public ChargingStationSimulatorManager(IConfiguration configuration, IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _centralSystemEndpoint = configuration.GetValue<string>("CentralSystemEndpoint")!;
        _hubEndpoint = configuration.GetValue<string>("HubEndpoint")!;
    }
    
    public bool TryAddChargingStation(Guid chargingStationId)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        
        var stateRepository = scope.ServiceProvider.GetRequiredService<IChargingStationStateRepository>();
        
        var chargingStation = new ChargingStation(chargingStationId, stateRepository, $"{_hubEndpoint}/ChargePointHub");
        
        return ChargingStations.TryAdd(chargingStationId, chargingStation);
    }
    
    public async Task StartChargingStationAsync(Guid chargingStationId, CancellationToken cancellationToken = default)
    {
        if (!ChargingStations.TryGetValue(chargingStationId, out var chargingStation))
            throw new InvalidOperationException();
        
        await chargingStation.InitializeAndStartAsync(_centralSystemEndpoint, cancellationToken);
    }
    
    public async Task StopChargingStationAsync(Guid chargingStationId, CancellationToken cancellationToken = default)
    {
        if (!ChargingStations.TryGetValue(chargingStationId, out var chargingStation))
            throw new InvalidOperationException();
        
        await chargingStation.DisconnectAsync(cancellationToken);
    }
    
    public bool RemoveChargingStation(Guid chargingStationId)
    {
        return ChargingStations.TryRemove(chargingStationId, out _);
    }
    
    public ChargingStationState GetState(Guid chargingStationId)
    {
        if (!ChargingStations.TryGetValue(chargingStationId, out var chargingStation))
            throw new InvalidOperationException();
        
        return chargingStation.State;
    }

    public async Task AuthorizeAsync(Guid id, string idTag, CancellationToken cancellationToken = default)
    {
        if (!ChargingStations.TryGetValue(id, out var chargingStation))
            throw new InvalidOperationException();
        
        await chargingStation.SendAuthorizeRequestAsync(idTag, cancellationToken);
    }

    public async Task StartTransactionAsync(Guid id, int connectorId, string authorizedIdTag)
    {
        if (!ChargingStations.TryGetValue(id, out var chargingStation))
            throw new InvalidOperationException();
        
        await chargingStation.SendStartTransactionRequestAsync(connectorId, authorizedIdTag);
    }

    public async Task ClearStateAsync(Guid id)
    {
        if (!ChargingStations.TryGetValue(id, out var chargingStation))
            throw new InvalidOperationException();
        
        await chargingStation.ClearStateAsync();
    }
}