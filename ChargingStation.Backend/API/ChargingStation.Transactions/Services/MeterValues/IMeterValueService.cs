using ChargingStation.Common.Messages_OCPP16;

namespace ChargingStation.Transactions.Services.MeterValues;

public interface IMeterValueService
{
    Task<MeterValuesResponse> ProcessMeterValueAsync(MeterValuesRequest request, Guid chargePointId, CancellationToken cancellationToken = default);
}