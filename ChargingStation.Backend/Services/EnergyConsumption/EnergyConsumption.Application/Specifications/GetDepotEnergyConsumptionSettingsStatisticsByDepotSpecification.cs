using ChargingStation.Common.Models.General.Requests;
using ChargingStation.Domain.Entities;
using ChargingStation.Infrastructure.Specifications;
using EnergyConsumption.Application.Models.Requests;

namespace EnergyConsumption.Application.Specifications;

public class GetDepotEnergyConsumptionSettingsStatisticsByDepotSpecification : Specification<DepotEnergyConsumptionSettings>
{
    public GetDepotEnergyConsumptionSettingsStatisticsByDepotSpecification(GetDepotEnergyConsumptionSettingsStatisticsRequest request)
    {
        if (request is { StartTime: not null, EndTime: not null })
            AddFilter(x => x.ValidFrom >= request.StartTime && x.ValidTo <= request.EndTime);
        else if (request.StartTime is not null)
            AddFilter(x => x.ValidFrom >= request.StartTime);
        else if (request.EndTime is not null)
            AddFilter(x => x.ValidTo <= request.EndTime);
        
        AddFilter(x => x.DepotId == request.DepotId);
        AddInclude(nameof(DepotEnergyConsumptionSettings.ChargePointsLimits));
        AddInclude(nameof(DepotEnergyConsumptionSettings.Intervals));
        AddSorting(new List<OrderPredicate>
        {
            new()
            {
                PropertyName = nameof(DepotEnergyConsumptionSettings.ValidFrom),
                OrderDirection = OrderDirection.Ascending
            }
        });
    }
}