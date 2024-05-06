using ChargingStation.Common.Models.OcppTags.Responses;
using Google.Protobuf.WellKnownTypes;
using OcppTags.Grpc;

namespace ChargingStation.OcppTags.Grpc.Extensions;

public static class ResponseExtensions
{
    public static OcppTagGrpcResponse ToGrpcResponse(this OcppTagResponse response)
    {
        return new OcppTagGrpcResponse
        {
            Id = response.Id.ToString(),
            TagId = response.TagId,
            ParentTagId = response.ParentTagId,
            ExpiryDate = response.ExpiryDate.HasValue ? Timestamp.FromDateTime(DateTime.SpecifyKind(response.ExpiryDate.Value, DateTimeKind.Utc)) : null,
            Blocked = response.Blocked ?? false,
            CreatedAt = Timestamp.FromDateTime(DateTime.SpecifyKind(response.CreatedAt, DateTimeKind.Utc)),
            UpdatedAt = response.UpdatedAt.HasValue ? Timestamp.FromDateTime(DateTime.SpecifyKind(response.UpdatedAt.Value, DateTimeKind.Utc)) : null
        };
    }
}