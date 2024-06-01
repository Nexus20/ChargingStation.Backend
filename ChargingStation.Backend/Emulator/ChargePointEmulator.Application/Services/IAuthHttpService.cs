namespace ChargePointEmulator.Application.Services;

internal interface IAuthHttpService
{
    Task<AuthHttpService.LoginResponse> AuthenticateAsync(CancellationToken cancellationToken = default);
}