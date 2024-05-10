using ChargePoints.Grpc.Protos;
using ChargingStation.Common.Models.ChargePoints.Responses;
using Google.Protobuf.WellKnownTypes;

namespace ChargePoints.Grpc.Extensions;

public static class ResponseExtensions
{
    public static ChargePointGrpcResponse ToGrpcResponse(this ChargePointResponse response)
    {
        return new ChargePointGrpcResponse
        {
            Id = response.Id.ToString(),
            DepotId = response.DepotId.ToString(),
            Description = response.Description,
            Iccid = response.Iccid,
            Imsi = response.Imsi,
            FirmwareVersion = response.FirmwareVersion,
            FirmwareUpdateTimestamp = response.FirmwareUpdateTimestamp.HasValue ? Timestamp.FromDateTime(DateTime.SpecifyKind(response.FirmwareUpdateTimestamp.Value, DateTimeKind.Utc)) : null,
            ChargePointVendor = response.ChargePointVendor,
            ChargePointModel = response.ChargePointModel,
            ChargePointSerialNumber = response.ChargePointSerialNumber,
            ChargeBoxSerialNumber = response.ChargeBoxSerialNumber,
            MeterType = response.MeterType,
            MeterSerialNumber = response.MeterSerialNumber,
            RegistrationStatus = response.RegistrationStatus,
            OcppProtocol = response.OcppProtocol,
            DiagnosticsTimestamp = response.DiagnosticsTimestamp.HasValue ? Timestamp.FromDateTime(DateTime.SpecifyKind(response.DiagnosticsTimestamp.Value, DateTimeKind.Utc)) : null,
            LastHeartbeat = response.LastHeartbeat.HasValue ? Timestamp.FromDateTime(DateTime.SpecifyKind(response.LastHeartbeat.Value, DateTimeKind.Utc)) : null,
            CreatedAt = Timestamp.FromDateTime(DateTime.SpecifyKind(response.CreatedAt, DateTimeKind.Utc)),
            UpdatedAt = response.UpdatedAt.HasValue ? Timestamp.FromDateTime(DateTime.SpecifyKind(response.UpdatedAt.Value, DateTimeKind.Utc)) : null
        };
    }
}