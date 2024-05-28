using ChargingStation.Common.Models.ChargePoints.Responses;
using ChargingStation.Common.Models.General.Requests;

namespace ChargePointEmulator.Application.Models;

public class CreateReservationRequest
{
    public Guid ChargePointId { get; set; }
    public int ConnectorId { get; set; }
    public Guid OcppTagId { get; set; }
    public DateTime StartDateTime { get; set; }
    public DateTime ExpiryDateTime { get; set; }
}

public class ChargePointSimulatorResponse : ChargePointResponse
{
    public bool SimulatorAdded { get; set; }
}

public class GetChargePointRequest : BaseCollectionRequest
{
    public Guid? DepotId { get; set; }
    public string? OcppProtocol { get; set; }
    public string? RegistrationStatus { get; set; }
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

    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}