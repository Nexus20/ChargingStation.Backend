namespace ChargingStation.Heartbeats.Services.Connectors;

public interface IChargePointHttpService
{
    Task<bool> GetByIdAsync(string chargePointId, CancellationToken cancellationToken = default);
}