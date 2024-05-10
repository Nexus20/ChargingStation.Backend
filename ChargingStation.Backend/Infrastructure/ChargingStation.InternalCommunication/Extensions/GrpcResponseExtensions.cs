using ChargePoints.Grpc.Protos;
using ChargingStation.Common.Models.ChargePoints.Responses;
using ChargingStation.Common.Models.Connectors.Responses;
using ChargingStation.Common.Models.DepotEnergyConsumption;
using ChargingStation.Common.Models.DepotEnergyConsumption.Dtos;
using ChargingStation.Common.Models.OcppTags.Responses;
using ChargingStation.Common.Models.Transactions.Responses;
using Connectors.Grpc.Protos;
using EnergyConsumption.Grpc.Protos;
using OcppTags.Grpc.Protos;
using Transactions.Grpc.Protos;

namespace ChargingStation.InternalCommunication.Extensions;

public static class GrpcResponseExtensions
{
    public static TransactionResponse ToResponse(this TransactionGrpcResponse grpcResponse)
    {
        return new TransactionResponse
        {
            TransactionId = grpcResponse.TransactionId
        };
    }
    
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
    
    public static DepotEnergyConsumptionSettingsResponse ToResponse(this DepotEnergyConsumptionSettingsGrpcResponse grpcResponse)
    {
        return new DepotEnergyConsumptionSettingsResponse
        {
            Id = Guid.Parse(grpcResponse.Id),
            DepotId = Guid.Parse(grpcResponse.DepotId),
            DepotEnergyLimit = grpcResponse.DepotEnergyLimit,
            ValidFrom = grpcResponse.ValidFrom.ToDateTime(),
            ValidTo = grpcResponse.ValidTo.ToDateTime(),
            CreatedAt = grpcResponse.CreatedAt.ToDateTime(),
            UpdatedAt = grpcResponse.UpdatedAt?.ToDateTime(),
            ChargePointsLimits = grpcResponse.ChargePointsLimits.Select(x => new ChargePointEnergyConsumptionSettingsDto
            {
                ChargePointId = Guid.Parse(x.ChargePointId),
                ChargePointEnergyLimit = x.ChargePointEnergyLimit
            }).ToList(),
            Intervals = grpcResponse.Intervals.Select(x => new EnergyConsumptionIntervalSettingsDto
            {
                EnergyLimit = x.EnergyLimit,
                StartTime = x.StartTime.ToDateTime(),
                EndTime = x.EndTime.ToDateTime()
            }).ToList()
        };
    }
}