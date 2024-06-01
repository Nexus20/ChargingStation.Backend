using ChargingStation.Common.Models.Transactions.Responses;
using Google.Protobuf.WellKnownTypes;
using Transactions.Grpc.Protos;

namespace Transactions.Grpc.Extensions;

public static class ResponseExtensions
{
    public static TransactionGrpcResponse ToGrpcResponse(this TransactionResponse response)
    {
        return new TransactionGrpcResponse
        {
            Id = response.Id.ToString(),
            TransactionId = response.TransactionId,
            ConnectorId = response.ConnectorId.ToString(),
            StartTagId = response.StartTagId,
            ReservationId = response.ReservationId?.ToString(),
            StartTime = Timestamp.FromDateTime(DateTime.SpecifyKind(response.StartTime, DateTimeKind.Utc)),
            StopTime = response.StopTime.HasValue ? Timestamp.FromDateTime(DateTime.SpecifyKind(response.StopTime.Value, DateTimeKind.Utc)) : null,
            StopTagId = response.StopTagId,
            StopReason = response.StopReason,
            CreatedAt = Timestamp.FromDateTime(DateTime.SpecifyKind(response.CreatedAt, DateTimeKind.Utc)),
            UpdatedAt = response.UpdatedAt.HasValue ? Timestamp.FromDateTime(DateTime.SpecifyKind(response.UpdatedAt.Value, DateTimeKind.Utc)) : null,
        };
    }
}