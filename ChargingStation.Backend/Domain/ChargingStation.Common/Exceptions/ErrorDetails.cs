using System.Text.Json;

namespace ChargingStation.Common.Exceptions;

public class ErrorDetails
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
    
    public string Error { get; set; }

    public ErrorDetails(string error)
    {
        Error = error;
    }
    
    public ErrorDetails(Exception exception)
    {
        Error = exception.Message;
    }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this, JsonSerializerOptions);
    }
}