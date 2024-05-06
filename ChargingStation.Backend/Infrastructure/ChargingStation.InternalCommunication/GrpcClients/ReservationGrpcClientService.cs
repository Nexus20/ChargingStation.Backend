using ChargingStation.Common.Models.Reservations.Requests;
using Reservations.Grpc;

namespace ChargingStation.InternalCommunication.GrpcClients;

public class ReservationGrpcClientService
{
    private readonly ReservationsGrpc.ReservationsGrpcClient _reservationsGrpcClient;

    public ReservationGrpcClientService(ReservationsGrpc.ReservationsGrpcClient reservationsGrpcClient)
    {
        _reservationsGrpcClient = reservationsGrpcClient;
    }

    public async Task UseReservationAsync(UseReservationRequest request, CancellationToken cancellationToken = default)
    {
        var grpcRequest = new UseReservationGrpcRequest
        {
            ChargePointId = request.ChargePointId.ToString(),
            ConnectorId = request.ConnectorId.ToString(),
            ReservationId = request.ReservationId
        };

        await _reservationsGrpcClient.UseReservationAsync(grpcRequest, cancellationToken: cancellationToken);
    }
}