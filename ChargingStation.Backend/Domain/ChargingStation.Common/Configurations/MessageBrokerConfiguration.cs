namespace ChargingStation.Common.Configurations;

public class MessageBrokerConfiguration
{
    public const string SectionName = "MessageBrokerSettings";
    
    public required string RabbitMqHost {get; set;}
    public required string RabbitMqUsername {get; set;}
    public required string RabbitMqPassword {get; set;}
    
    public string GetConnectionString()
    {
        return $"amqp://{RabbitMqUsername}:{RabbitMqPassword}@{RabbitMqHost}:5672";
    }
}