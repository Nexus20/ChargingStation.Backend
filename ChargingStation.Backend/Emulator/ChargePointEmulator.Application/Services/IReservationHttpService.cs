using ChargePointEmulator.Application.Models;

namespace ChargePointEmulator.Application.Services;

public interface IReservationHttpService
{
    Task CreateReservationAsync(CreateReservationRequest request,
        CancellationToken cancellationToken = default);
}