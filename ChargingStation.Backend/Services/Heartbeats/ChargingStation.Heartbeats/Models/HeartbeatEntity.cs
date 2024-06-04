using Azure;
using ChargingStation.TableStorage.Models;

namespace ChargingStation.Heartbeats.Models;

public class HeartbeatEntity : BaseTableStorageEntity
{
    public DateTimeOffset CurrentTime { get; set; }

    public HeartbeatEntity(string partitionKey, ETag eTag, DateTimeOffset currentTime) : base(partitionKey, eTag)
    {
        CurrentTime = currentTime;
    }

    public HeartbeatEntity(string partitionKey, DateTimeOffset currentTime) : base(partitionKey)
    {
        CurrentTime = currentTime;
    }
}