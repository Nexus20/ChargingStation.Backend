﻿using ChargingStation.Domain.Abstract;

namespace ChargingStation.Connectors.Models.Responses;

public class ConnectorResponse : ITimeMarkable
{
    public Guid Id { get; set; }
    public Guid ChargePointId { get; set; }
    public int ConnectorId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}