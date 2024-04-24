﻿using ChargingStation.Common.Models.Depots.Responses;

namespace ChargingStation.Aggregator.Models.Responses;

public class DepotAggregatedResponse : DepotResponse
{
    public ChargePointsStatisticsResponse ChargePointsStatistics { get; set; } = new();
}