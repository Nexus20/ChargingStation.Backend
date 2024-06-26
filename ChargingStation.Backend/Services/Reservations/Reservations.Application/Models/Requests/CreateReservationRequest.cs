﻿namespace Reservations.Application.Models.Requests;

public class CreateReservationRequest
{
    public Guid ChargePointId { get; set; }
    public int ConnectorId { get; set; }
    public Guid OcppTagId { get; set; }
    public DateTime StartDateTime { get; set; }
    public DateTime ExpiryDateTime { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
}