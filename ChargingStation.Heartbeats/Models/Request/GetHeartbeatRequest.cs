namespace ChargingStation.Heartbeats.Models.Request
{
    public class GetHeartbeatRequest
    {
        public required string PartitionKey { get; set; }
        public required string RowKey { get; set; }
    }
}
