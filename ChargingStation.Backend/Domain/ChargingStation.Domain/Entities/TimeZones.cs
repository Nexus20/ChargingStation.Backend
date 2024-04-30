﻿using ChargingStation.Domain.Abstract;

namespace ChargingStation.Domain.Entities;

public class TimeZones : Entity
{
    public required string Name { get; set; }
    public required TimeSpan BaseUtcOffset { get; set; }
    public required string IanaId { get; set; }
    public required string WindowsId { get; set; }

    public List<Depot>? Depots { get; set; }
}

