namespace ChargingStation.Heartbeats.Services.ChargePoints;

public interface IChargePointHttpService
{
    Task<bool> GetByIdAsync(string chargePointId, CancellationToken cancellationToken = default);
}