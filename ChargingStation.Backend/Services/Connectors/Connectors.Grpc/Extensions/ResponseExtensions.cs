using ChargingStation.Common.Models.Connectors.Responses;
using Connectors.Grpc.Protos;
using Google.Protobuf.WellKnownTypes;

namespace Connectors.Grpc.Extensions;

public static class ResponseExtensions
{
    public static ConnectorGrpcResponse ToGrpcResponse(this ConnectorResponse response)
    {
        return new ConnectorGrpcResponse
        {
            Id = response.Id.ToString(),
            ChargePointId = response.ChargePointId.ToString(),
            ConnectorId = response.ConnectorId,
            CreatedAt = Timestamp.FromDateTime(DateTime.SpecifyKind(response.CreatedAt, DateTimeKind.Utc)),
            CurrentStatus = response.CurrentStatus != null ? new ConnectorStatusGrpcResponse
            {
                ConnectorId = response.CurrentStatus.ConnectorId.ToString(),
                CurrentStatus = response.CurrentStatus.CurrentStatus,
                ErrorCode = response.CurrentStatus.ErrorCode,
                Info = response.CurrentStatus.Info,
                StatusUpdatedTimestamp = response.CurrentStatus.StatusUpdatedTimestamp.HasValue ? Timestamp.FromDateTime(response.CurrentStatus.StatusUpdatedTimestamp.Value) : null,
                VendorErrorCode = response.CurrentStatus.VendorErrorCode,
                VendorId = response.CurrentStatus.VendorId
            } : null
        };
    }
}