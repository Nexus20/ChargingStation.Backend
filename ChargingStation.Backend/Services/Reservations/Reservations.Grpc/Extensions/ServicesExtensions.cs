using Reservations.Application.Extensions;

namespace Reservations.Grpc.Extensions;

public static class ServicesExtensions
{
    public static IServiceCollection AddReservationGrpcServices(this IServiceCollection services)
    {
        services.AddGrpc();
        services.AddReservationApplicationBaseServices();

        return services;
    }
}