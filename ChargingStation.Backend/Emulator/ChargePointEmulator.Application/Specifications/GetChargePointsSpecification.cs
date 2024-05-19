using ChargePointEmulator.Application.Models;
using ChargingStation.Domain.Entities;
using ChargingStation.Infrastructure.Specifications;

namespace ChargePointEmulator.Application.Specifications;

public class GetChargePointsSpecification : Specification<ChargePoint>
{
    public GetChargePointsSpecification(GetChargePointRequest request)
    {
        AddFilters(request);

        if (request.OrderPredicates.Count != 0)
            AddSorting(request.OrderPredicates);
    }

    private void AddFilters(GetChargePointRequest request)
    {
        if(request.DepotId.HasValue)
            AddFilter(с => с.DepotId == request.DepotId);
        
        if (!string.IsNullOrEmpty(request.OcppProtocol))
            AddFilter(с => с.OcppProtocol != null && с.OcppProtocol.Contains(request.OcppProtocol));

        if (!string.IsNullOrEmpty(request.ChargePointVendor))
            AddFilter(с => с.ChargePointVendor != null && с.ChargePointVendor.Contains(request.ChargePointVendor));

        if (!string.IsNullOrEmpty(request.ChargePointModel))
            AddFilter(с => с.ChargePointModel != null && с.ChargePointModel.Contains(request.ChargePointModel));

        if (!string.IsNullOrEmpty(request.ChargePointSerialNumber))
            AddFilter(с => с.ChargePointSerialNumber != null && с.ChargePointSerialNumber.Contains(request.ChargePointSerialNumber));

        if (!string.IsNullOrEmpty(request.ChargeBoxSerialNumber))
            AddFilter(с => с.ChargeBoxSerialNumber != null && с.ChargeBoxSerialNumber.Contains(request.ChargeBoxSerialNumber));

        if (!string.IsNullOrEmpty(request.FirmwareVersion))
            AddFilter(с => с.FirmwareVersion != null && с.FirmwareVersion.Contains(request.FirmwareVersion));

        if (!string.IsNullOrEmpty(request.Iccid))
            AddFilter(с => с.Iccid != null && с.Iccid.Contains(request.Iccid));

        if (!string.IsNullOrEmpty(request.Imsi))
            AddFilter(с => с.Imsi != null && с.Imsi.Contains(request.Imsi));

        if (!string.IsNullOrEmpty(request.MeterType))
            AddFilter(с => с.MeterType != null && с.MeterType.Contains(request.MeterType));

        if (!string.IsNullOrEmpty(request.MeterSerialNumber))
            AddFilter(с => с.MeterSerialNumber != null && с.MeterSerialNumber.Contains(request.MeterSerialNumber));

        if (!string.IsNullOrEmpty(request.Description))
            AddFilter(с => с.Description != null && с.Description.Contains(request.Description));

        if (!string.IsNullOrEmpty(request.RegistrationStatus))
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