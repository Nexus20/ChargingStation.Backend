using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ChargePointEmulator.Application.State;

public class ChargingStationStateEntity
{
    [BsonId]
    public required string Id { get; init; }
    [BsonRepresentation(BsonType.String)]
    public required string JsonState { get; set; }
}