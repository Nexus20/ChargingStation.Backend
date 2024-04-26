using ChargingStation.Mailing.Configuration;
using ChargingStation.Mailing.Services;
using Mailjet.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ChargingStation.Mailing.Extensions;

public static class ServicesExtensions
{
    public static IServiceCollection AddMailingServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MailjetConfiguration>(options => configuration.GetSection(MailjetConfiguration.SectionName).Bind(options));
        
        services.AddHttpClient<IMailjetClient, MailjetClient>(client =>
        {
            var mailjetConfiguration = configuration.GetSection(MailjetConfiguration.SectionName).Get<MailjetConfiguration>()!;
            
            client.UseBasicAuthentication(mailjetConfiguration.ApiKey, mailjetConfiguration.ApiSecret);
        });
        
        services.AddScoped<IEmailService, EmailService>();

        return services;
    }
}