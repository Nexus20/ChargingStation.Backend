﻿using ChargingStation.Domain.Entities;
using ChargingStation.Infrastructure.Specifications;
using ChargingStation.Reservations.Models.Requests;

namespace ChargingStation.Reservations.Specifications;

public class GetReservationsSpecification : Specification<Reservation>
{
    public GetReservationsSpecification(GetReservationsRequest request)
    {
        AddFilters(request);
    }
    
    private void AddFilters(GetReservationsRequest request)
    {
        if (request.ChargePointId.HasValue)
        {
            AddFilter(r => r.ChargePointId == request.ChargePointId);
        }
        
        if (request.ConnectorId.HasValue)
        {
            AddFilter(r => r.ConnectorId == request.ConnectorId);
        }
        
        if (request.TagId.HasValue)
        {
            AddFilter(r => r.TagId == request.TagId);
        }
        
        if (request.StartDateTime.HasValue)
        {
            AddFilter(r => r.StartDateTime >= request.StartDateTime);
        }
        
        if (request.ExpiryDateTime.HasValue)
        {
            AddFilter(r => r.ExpiryDateTime <= request.ExpiryDateTime);
        }
        
        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            AddFilter(r => r.Status == request.Status);
        }
        
        AddFilter(r => r.IsCancelled == request.IsCancelled);
    }
}