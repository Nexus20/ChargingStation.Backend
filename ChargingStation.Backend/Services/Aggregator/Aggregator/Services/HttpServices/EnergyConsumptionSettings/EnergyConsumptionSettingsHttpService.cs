﻿using System.Net;
using ChargingStation.Common.Models.DepotEnergyConsumption;

namespace Aggregator.Services.HttpServices.EnergyConsumptionSettings;

public class EnergyConsumptionSettingsHttpService : IEnergyConsumptionSettingsHttpService
{
    private readonly HttpClient _httpClient;

    public EnergyConsumptionSettingsHttpService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<DepotEnergyConsumptionSettingsResponse?> GetEnergyConsumptionSettingsByDepotAsync(Guid depotId, CancellationToken cancellationToken = default)
    {
        var requestUri = $"getByDepot/{depotId}";
        
        var response = await _httpClient.GetAsync(requestUri, cancellationToken);
        
        response.EnsureSuccessStatusCode();
        
        if(response.StatusCode == HttpStatusCode.NoContent)
            return null;
        
        var result = await response.Content.ReadFromJsonAsync<DepotEnergyConsumptionSettingsResponse>(cancellationToken: cancellationToken);
        
        return result;
    }

    public async Task<DepotEnergyConsumptionSettingsResponse?> GetByChargingStationIdAsync(Guid chargePointId, CancellationToken cancellationToken = default)
    {
        var requestUri = $"getByChargingStation/{chargePointId}";
        
        var response = await _httpClient.GetAsync(requestUri, cancellationToken);
        
        response.EnsureSuccessStatusCode();
        
        if(response.StatusCode == HttpStatusCode.NoContent)
            return null;
        
        var result = await response.Content.ReadFromJsonAsync<DepotEnergyConsumptionSettingsResponse>(cancellationToken: cancellationToken);
        
        return result;
    }
}