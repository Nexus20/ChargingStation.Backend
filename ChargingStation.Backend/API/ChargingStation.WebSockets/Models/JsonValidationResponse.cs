using Newtonsoft.Json.Schema;

namespace ChargingStation.WebSockets.Models;

public class JsonValidationResponse
{
    public bool Valid { get; set; }

    public List<ValidationError> Errors { get; set; }

    public string CustomErrors { get; set; }
}