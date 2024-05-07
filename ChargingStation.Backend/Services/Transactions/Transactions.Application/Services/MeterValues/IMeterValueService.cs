using ChargingStation.Common.Messages_OCPP16.Requests;
using ChargingStation.Common.Messages_OCPP16.Responses;

namespace Transactions.Application.Services.MeterValues;

public interface IMeterValueService
{
    Task<MeterValuesResponse> ProcessMeterValueAsync(MeterValuesRequest request, Guid chargePointId, CancellationToken cancellationToken = default);
}