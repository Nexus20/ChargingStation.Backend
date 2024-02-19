using ChargingStation.Common.Models.Requests;
using ChargingStation.Domain.Entities;

namespace ChargingStation.ChargePoints.Models.Requests
{
    public class GetChargePointRequest : BaseCollectionRequest
    {
        public Guid DepotId { get; set; }
        public string OcppProtocol { get; set; } = null!;
        public ChargePointRegistrationStatus? RegistrationStatus { get; set; }
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
}
