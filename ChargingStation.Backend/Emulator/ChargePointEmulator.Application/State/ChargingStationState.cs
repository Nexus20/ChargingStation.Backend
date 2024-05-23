using System.Collections.Concurrent;
using ChargingStation.Common.Messages_OCPP16;
using ChargingStation.Common.Messages_OCPP16.Responses;
using ChargingStation.Common.Messages_OCPP16.Responses.Enums;
using ChargingStation.Common.Models.General;
using Newtonsoft.Json;

namespace ChargePointEmulator.Application.State;

public class ChargingStationState
{
    [JsonIgnore]
    public DateTime? LastMessageReceived { get; set; }
    public required string Id { get; init; }
    public ConcurrentDictionary<string, OcppMessage> PendingRequests { get; init; } = new();
    public ConcurrentDictionary<int, ConnectorState> Connectors { get; init; } = new();
    public ConcurrentDictionary<int, ReservationState> Reservations { get; init; } = new();
    public ConcurrentDictionary<string, IdTagInfo> AuthorizedOcppTags { get; init; } = new();
    [JsonIgnore]
    public ConcurrentDictionary<string, TaskCompletionSource<AuthorizeResponse>> PendingAuthorizeRequests { get; } = new();
    public BootNotificationResponseStatus BootNotificationStatus { get; set; }
}