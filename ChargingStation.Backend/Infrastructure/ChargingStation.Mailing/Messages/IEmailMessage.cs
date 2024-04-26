namespace ChargingStation.Mailing.Messages;

public interface IEmailMessage
{
    string Subject { get; }
    string GetTextPart();
    string GetHtmlPart();
}