using ChargingStation.ChargePoints.Models.Requests;
using ChargingStation.Domain.Entities;
using ChargingStation.Infrastructure.Specifications;

namespace ChargingStation.ChargePoints.Specifications;

public class GetChargePointsSpecification : Specification<ChargePoint>
{
    public GetChargePointsSpecification(GetChargePoint request)
    {
        AddFilters(request);

        if (request.OrderPredicates.Count != 0)
            AddSorting(request.OrderPredicates);
    }

    private void AddFilters(GetChargePoint request)
    {
        if (!string.IsNullOrEmpty(request.OcppProtocol))
            AddFilter(с => с.OcppProtocol.Contains(request.OcppProtocol));

        if (!string.IsNullOrEmpty(request.ChargePointVendor))
            AddFilter(с => с.ChargePointVendor.Contains(request.ChargePointVendor));

        if (!string.IsNullOrEmpty(request.ChargePointModel))
            AddFilter(с => с.ChargePointModel.Contains(request.ChargePointModel));

        if (!string.IsNullOrEmpty(request.ChargePointSerialNumber))
            AddFilter(с => с.ChargePointSerialNumber.Contains(request.ChargePointSerialNumber));

        if (!string.IsNullOrEmpty(request.ChargeBoxSerialNumber))
            AddFilter(с => с.ChargeBoxSerialNumber.Contains(request.ChargeBoxSerialNumber));

        if (!string.IsNullOrEmpty(request.FirmwareVersion))
            AddFilter(с => с.FirmwareVersion.Contains(request.FirmwareVersion));

        if (!string.IsNullOrEmpty(request.Iccid))
            AddFilter(с => с.Iccid.Contains(request.Iccid));

        if (!string.IsNullOrEmpty(request.Imsi))
            AddFilter(с => с.Imsi.Contains(request.Imsi));

        if (!string.IsNullOrEmpty(request.MeterType))
            AddFilter(с => с.MeterType.Contains(request.MeterType));

        if (!string.IsNullOrEmpty(request.MeterSerialNumber))
            AddFilter(с => с.MeterSerialNumber.Contains(request.MeterSerialNumber));

        if (!string.IsNullOrEmpty(request.Description))
            AddFilter(с => с.Description.Contains(request.Description));

        if (request.RegistrationStatus.HasValue)
            AddFilter(с => с.RegistrationStatus == request.RegistrationStatus);

        if (request.FirmwareUpdateTimestamp.HasValue)
            AddFilter(с => с.FirmwareUpdateTimestamp == request.FirmwareUpdateTimestamp);

        if (request.DiagnosticsTimestamp.HasValue)
            AddFilter(с => с.DiagnosticsTimestamp == request.DiagnosticsTimestamp);

        if (request.LastHeartbeat.HasValue)
            AddFilter(с => с.LastHeartbeat == request.LastHeartbeat);

        if (request.CreatedAt.HasValue)
            AddFilter(с => с.CreatedAt == request.CreatedAt);

        if (request.UpdatedAt.HasValue)
            AddFilter(с => с.UpdatedAt == request.UpdatedAt);
    }
}