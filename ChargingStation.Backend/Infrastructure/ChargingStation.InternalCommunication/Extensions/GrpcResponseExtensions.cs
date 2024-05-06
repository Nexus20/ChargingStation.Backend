using ChargePoints.Grpc;
using ChargingStation.Common.Models.ChargePoints.Responses;
using ChargingStation.Common.Models.Connectors.Responses;
using ChargingStation.Common.Models.OcppTags.Responses;
using Connectors.Grpc;
using OcppTags.Grpc;

namespace ChargingStation.InternalCommunication.Extensions;

public static class GrpcResponseExtensions
{
    public static OcppTagResponse ToResponse(this OcppTagGrpcResponse grpcResponse)
    {
        return new OcppTagResponse
        {
            Id = Guid.Parse(grpcResponse.Id),
            TagId = grpcResponse.TagId,
            CreatedAt = grpcResponse.CreatedAt.ToDateTime(),
            UpdatedAt = grpcResponse.UpdatedAt?.ToDateTime()
        };
    }
    
    public static ConnectorResponse ToResponse(this ConnectorGrpcResponse grpcResponse)
    {
        return new ConnectorResponse
        {
            Id = Guid.Parse(grpcResponse.Id),
            ChargePointId = Guid.Parse(grpcResponse.ChargePointId),
            ConnectorId = grpcResponse.ConnectorId,
            CreatedAt = grpcResponse.CreatedAt.ToDateTime(),
            CurrentStatus = grpcResponse.CurrentStatus != null ? new ConnectorStatusResponse
            {
                ConnectorId = Guid.Parse(grpcResponse.CurrentStatus.ConnectorId),
                CurrentStatus = grpcResponse.CurrentStatus.CurrentStatus,
                ErrorCode = grpcResponse.CurrentStatus.ErrorCode,
                Info = grpcResponse.CurrentStatus.Info,
                StatusUpdatedTimestamp = grpcResponse.CurrentStatus.StatusUpdatedTimestamp?.ToDateTime(),
                VendorErrorCode = grpcResponse.CurrentStatus.VendorErrorCode,
                VendorId = grpcResponse.CurrentStatus.VendorId
            } : null
        };
    }
    
    public static ChargePointResponse ToResponse(this ChargePointGrpcResponse grpcResponse)
    {
        return new ChargePointResponse
        {
            Id = Guid.Parse(grpcResponse.Id),
            DepotId = Guid.Parse(grpcResponse.DepotId),
            Description = grpcResponse.Description,
            Iccid = grpcResponse.Iccid,
            Imsi = grpcResponse.Imsi,
            FirmwareVersion = grpcResponse.FirmwareVersion,
            FirmwareUpdateTimestamp = grpcResponse.FirmwareUpdateTimestamp?.ToDateTime(),
            ChargePointVendor = grpcResponse.ChargePointVendor,
            ChargePointModel = grpcResponse.ChargePointModel,
            ChargePointSerialNumber = grpcResponse.ChargePointSerialNumber,
            ChargeBoxSerialNumber = grpcResponse.ChargeBoxSerialNumber,
            MeterType = grpcResponse.MeterType,
            MeterSerialNumber = grpcResponse.MeterSerialNumber,
            RegistrationStatus = grpcResponse.RegistrationStatus,
            OcppProtocol = grpcResponse.OcppProtocol,
            DiagnosticsTimestamp = grpcResponse.DiagnosticsTimestamp?.ToDateTime(),
            LastHeartbeat = grpcResponse.LastHeartbeat?.ToDateTime(),
            CreatedAt = grpcResponse.CreatedAt.ToDateTime(),
            UpdatedAt = grpcResponse.UpdatedAt?.ToDateTime()
        };
    }
}