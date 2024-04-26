namespace ChargingStation.Mailing.Configuration;

public class MailjetConfiguration
{
    public const string SectionName = "Mailjet";

    public required string ApiKey { get; set; }
    public required string ApiSecret { get; set; }
    public required string EmailFrom { get; set; }
}