using ChargingStation.Common.Models.Reservations.Requests;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Reservations.Application.Services.Reservations;
using Reservations.Grpc.Protos;

namespace Reservations.Grpc.Services;

public class ReservationGrpcService : ReservationsGrpc.ReservationsGrpcBase
{
    private readonly IBaseReservationService _reservationService;
    
    public ReservationGrpcService(IBaseReservationService reservationService)
    {
        _reservationService = reservationService;
    }

    public override async Task<Empty> UseReservation(UseReservationGrpcRequest request, ServerCallContext context)
    {
        var useReservationRequest = new UseReservationRequest
        {
            ChargePointId = Guid.Parse(request.ChargePointId),
            ConnectorId = Guid.Parse(request.ConnectorId),
            ReservationId = request.ReservationId
        };
        
        await _reservationService.UseReservationAsync(useReservationRequest);
        
        return new Empty();
    }
}