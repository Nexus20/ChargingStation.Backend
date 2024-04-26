using ChargingStation.Mailing.Messages;

namespace ChargingStation.Mailing.Services;

public interface IEmailService
{
    Task SendMessageAsync(IEmailMessage message, string to = "yevhen.chubarov@nure.ua",
        CancellationToken cancellationToken = default);
}