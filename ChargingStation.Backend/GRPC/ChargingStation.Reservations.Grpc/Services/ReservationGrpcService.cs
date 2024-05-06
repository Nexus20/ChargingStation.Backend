using ChargingStation.Common.Models.Reservations.Requests;
using ChargingStation.Reservations.Services.Reservations;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Reservations.Grpc;

namespace ChargingStation.Reservations.Grpc.Services;

public class ReservationGrpcService : ReservationsGrpc.ReservationsGrpcBase
{
    private readonly IReservationService _reservationService;
    
    public ReservationGrpcService(IReservationService reservationService)
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