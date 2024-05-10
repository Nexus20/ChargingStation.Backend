using ChargingStation.Common.Models.DepotEnergyConsumption;
using ChargingStation.Common.Models.DepotEnergyConsumption.Dtos;
using EnergyConsumption.Grpc.Protos;
using Google.Protobuf.WellKnownTypes;

namespace EnergyConsumption.Grpc.Extensions;

public static class ResponseExtensions
{
    public static DepotEnergyConsumptionSettingsGrpcResponse ToGrpcResponse(this DepotEnergyConsumptionSettingsResponse response)
    {
        return new DepotEnergyConsumptionSettingsGrpcResponse
        {
            Id = response.Id.ToString(),
            DepotId = response.DepotId.ToString(),
            DepotEnergyLimit = response.DepotEnergyLimit,
            ValidFrom = Timestamp.FromDateTime(DateTime.SpecifyKind(response.ValidFrom, DateTimeKind.Utc)),
            ValidTo = Timestamp.FromDateTime(DateTime.SpecifyKind(response.ValidTo, DateTimeKind.Utc)),
            ChargePointsLimits = { response.ChargePointsLimits.Select(x => x.ToGrpcMessage()) },
            Intervals = { response.Intervals.Select(x => x.ToGrpcMessage()) },
            CreatedAt = Timestamp.FromDateTime(DateTime.SpecifyKind(response.CreatedAt, DateTimeKind.Utc)),
            UpdatedAt = response.UpdatedAt.HasValue
                ? Timestamp.FromDateTime(DateTime.SpecifyKind(response.UpdatedAt.Value, DateTimeKind.Utc))
                : null
        };
    }
    
    private static ChargePointEnergyConsumptionSettingsGrpcMessage ToGrpcMessage(this ChargePointEnergyConsumptionSettingsDto dto)
    {
        return new ChargePointEnergyConsumptionSettingsGrpcMessage
        {
            ChargePointId = dto.ChargePointId.ToString(),
            ChargePointEnergyLimit = dto.ChargePointEnergyLimit
        };
    }
    
    private static EnergyConsumptionIntervalSettingsGrpcMessage ToGrpcMessage(this EnergyConsumptionIntervalSettingsDto dto)
    {
        return new EnergyConsumptionIntervalSettingsGrpcMessage
        {
            EnergyLimit = dto.EnergyLimit,
            StartTime = Timestamp.FromDateTime(DateTime.SpecifyKind(dto.StartTime, DateTimeKind.Utc)),
            EndTime = Timestamp.FromDateTime(DateTime.SpecifyKind(dto.EndTime, DateTimeKind.Utc))
        };
    }
}