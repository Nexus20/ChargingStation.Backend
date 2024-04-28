using ChargingStation.Mailing.Configuration;
using ChargingStation.Mailing.Messages;
using Mailjet.Client;
using Mailjet.Client.Resources;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace ChargingStation.Mailing.Services;

public class EmailService : IEmailService
{
    private readonly IMailjetClient _mailjetClient;
    private readonly MailjetConfiguration _mailingConfiguration;

    public EmailService(IMailjetClient mailjetClient, IOptions<MailjetConfiguration> mailingSettingsOptions)
    {
        _mailjetClient = mailjetClient;
        _mailingConfiguration = mailingSettingsOptions.Value;
    }

    public async Task SendMessageAsync(IEmailMessage message, string to = "yevhen.chubarov@nure.ua", CancellationToken cancellationToken = default)
    {
        var request = new MailjetRequest
            {
                Resource = SendV31.Resource,
            }
            .Property(Send.Messages, new JArray
            {
                new JObject
                {
                    {
                        "From",
                        new JObject
                        {
                            { "Email", _mailingConfiguration.EmailFrom },
                            { "Name", "E-Charge Hub" }
                        }
                    },
                    {
                        "To",
                        new JArray
                        {
                            new JObject
                            {
                                { "Email", to }
                            }
                        }
                    },
                    {
                        "Subject",
                        message.Subject
                    },
                    {
                        "TextPart",
                        message.GetTextPart()
                    },
                    {
                        "HTMLPart",
                        message.GetHtmlPart()
                    }
                }
            });

        var response = await _mailjetClient.PostAsync(request);
        
        if (!response.IsSuccessStatusCode)
        {
            var responseContent = response.Content.ToString();
            throw new Exception($"Failed to send email. Response: {responseContent}");
        }
    }
}