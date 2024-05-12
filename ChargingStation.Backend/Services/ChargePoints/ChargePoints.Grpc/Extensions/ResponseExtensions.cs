using ChargePoints.Grpc.Protos;
using ChargingStation.Common.Models.ChargePoints.Responses;
using Google.Protobuf.WellKnownTypes;

namespace ChargePoints.Grpc.Extensions;

public static class ResponseExtensions
{
    public static ChargePointGrpcResponse ToGrpcResponse(this ChargePointResponse response)
    {
        var grpcResponse = new ChargePointGrpcResponse();
        grpcResponse.Id = response.Id.ToString();
        grpcResponse.DepotId = response.DepotId.ToString();
        grpcResponse.Description = response.Description;
        grpcResponse.Iccid = response.Iccid;
        grpcResponse.Imsi = response.Imsi;
        grpcResponse.FirmwareVersion = response.FirmwareVersion;
        grpcResponse.FirmwareUpdateTimestamp = response.FirmwareUpdateTimestamp.HasValue ? Timestamp.FromDateTime(DateTime.SpecifyKind(response.FirmwareUpdateTimestamp.Value, DateTimeKind.Utc)) : null;
        grpcResponse.ChargePointVendor = response.ChargePointVendor;
        grpcResponse.ChargePointModel = response.ChargePointModel;
        grpcResponse.ChargePointSerialNumber = response.ChargePointSerialNumber;
        grpcResponse.ChargeBoxSerialNumber = response.ChargeBoxSerialNumber;
        grpcResponse.MeterType = response.MeterType;
        grpcResponse.MeterSerialNumber = response.MeterSerialNumber;
        grpcResponse.RegistrationStatus = response.RegistrationStatus;
        grpcResponse.OcppProtocol = response.OcppProtocol;
        grpcResponse.DiagnosticsTimestamp = response.DiagnosticsTimestamp.HasValue ? Timestamp.FromDateTime(DateTime.SpecifyKind(response.DiagnosticsTimestamp.Value, DateTimeKind.Utc)) : null;
        grpcResponse.LastHeartbeat = response.LastHeartbeat.HasValue ? Timestamp.FromDateTime(DateTime.SpecifyKind(response.LastHeartbeat.Value, DateTimeKind.Utc)) : null;
        grpcResponse.CreatedAt = Timestamp.FromDateTime(DateTime.SpecifyKind(response.CreatedAt, DateTimeKind.Utc));
        grpcResponse.UpdatedAt = response.UpdatedAt.HasValue ? Timestamp.FromDateTime(DateTime.SpecifyKind(response.UpdatedAt.Value, DateTimeKind.Utc)) : null;
        return grpcResponse;
    }
}