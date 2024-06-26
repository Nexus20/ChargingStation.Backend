﻿using ChargingStation.Common.Models.Abstract;
using ChargingStation.Domain.Abstract;

namespace ChargingStation.Domain.Entities;

public class ChargePoint : Entity, ITimeMarkable
{
    public required Guid DepotId { get; set; }
    public required Depot Depot { get; set; }
    public required string Name { get; set; }
    public string? OcppProtocol { get; set; }
    public required string RegistrationStatus { get; set; } = "Created";
    public string? ChargePointVendor { get; set; }
    public string? ChargePointModel { get; set; }
    public string? ChargePointSerialNumber { get; set; }
    public string? ChargeBoxSerialNumber { get; set; }
    public string? FirmwareVersion { get; set; }
    public DateTime? FirmwareUpdateTimestamp { get; set; }
    public string? Iccid { get; set; }
    public string? Imsi { get; set; }
    public string? MeterType { get; set; }
    public string? MeterSerialNumber { get; set; }
    
    public DateTime? DiagnosticsTimestamp { get; set; }
    public DateTime? LastHeartbeat { get; set; }
    
    public string? Description { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    public List<OcppTransaction>? Transactions { get; set; }
    public List<Connector>? Connectors { get; set; }
    public List<Reservation>? Reservations { get; set; }
    
    public List<ChargePointEnergyConsumptionSettings>? EnergyConsumptionSettings { get; set; }
}