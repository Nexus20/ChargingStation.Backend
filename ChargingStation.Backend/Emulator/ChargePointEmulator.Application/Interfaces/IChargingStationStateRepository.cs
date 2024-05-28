using ChargePointEmulator.Application.State;

namespace ChargePointEmulator.Application.Interfaces;

public interface IChargingStationStateRepository
{
    Task InsertAsync(ChargingStationState state, CancellationToken cancellationToken = default);
    Task<ChargingStationState?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task UpdateAsync(ChargingStationState state, CancellationToken cancellationToken = default);
    Task DeleteAsync(string id, CancellationToken cancellationToken = default);
    Task<List<ChargingStationState>> GetAllAsync(CancellationToken cancellationToken = default);
}