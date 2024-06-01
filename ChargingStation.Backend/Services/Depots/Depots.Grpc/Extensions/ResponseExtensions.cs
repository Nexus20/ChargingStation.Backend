using ChargingStation.Common.Models.Depots.Responses;
using Depots.Grpc.Protos;
using Google.Protobuf.WellKnownTypes;

namespace Depots.Grpc.Extensions;

public static class ResponseExtensions
{
    public static DepotGrpcResponse ToGrpcResponse(this DepotResponse response)
    {
        return new DepotGrpcResponse
        {
            Id = response.Id.ToString(),
            Name = response.Name,
            Country = response.Country,
            City = response.City,
            Street = response.Street,
            Building = response.Building,
            CreatedAt = Timestamp.FromDateTime(DateTime.SpecifyKind(response.CreatedAt, DateTimeKind.Utc)),
            UpdatedAt = response.UpdatedAt.HasValue ? Timestamp.FromDateTime(DateTime.SpecifyKind(response.UpdatedAt.Value, DateTimeKind.Utc)) : null,
            BaseUtcOffset = Duration.FromTimeSpan(response.BaseUtcOffset),
            Status = (int)response.Status,
            Description = response.Description,
            Latitude = response.Latitude,
            Longitude = response.Longitude,
            Email = response.Email,
            PhoneNumber = response.PhoneNumber,
            IanaId = response.IanaId
        };
    }
}