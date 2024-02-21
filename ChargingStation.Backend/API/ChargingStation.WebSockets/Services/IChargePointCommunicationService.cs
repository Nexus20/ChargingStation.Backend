﻿using ChargingStation.Common.Models;

namespace ChargingStation.WebSockets.Services;

public interface IChargePointCommunicationService
{
    Task CheckChargePointPresenceAsync(Guid chargePointId, CancellationToken cancellationToken = default);
    Task HandleMessageAsync(OcppMessage message, Guid chargePointId, CancellationToken cancellationToken = default);
}