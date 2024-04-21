using System.Reflection;
using ChargingStation.Common.Models.General;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema;

namespace ChargingStation.WebSockets.OcppMessageHandlers.Abstract;

public abstract class OcppMessageHandler : IOcppMessageHandler
{
    protected OcppMessageHandler(IConfiguration configuration, ILogger<OcppMessageHandler> logger)
    {
        _configuration = configuration;
        Logger = logger;
    }

    public abstract string ProtocolVersion { get; }
    public abstract string MessageType { get; }
    public virtual bool IsResponseHandler => false;
    
    private readonly IConfiguration _configuration;
    protected ILogger<OcppMessageHandler> Logger { get; }
    
    public abstract Task HandleAsync(OcppMessage inputMessage, Guid chargePointId, CancellationToken cancellationToken = default);

    protected T DeserializeMessage<T>(OcppMessage msg)
    {
        var path = Assembly.GetExecutingAssembly().Location;
        var codeBase = Path.GetDirectoryName(path);

        var validateMessages = _configuration.GetValue<bool>("ValidateMessages", false);

        string schemaJson = null;
        if (validateMessages &&
            !string.IsNullOrEmpty(codeBase) &&
            Directory.Exists(codeBase))
        {
            var msgTypeName = typeof(T).Name;
            var filename = Path.Combine(codeBase, $"Schema{ProtocolVersion}", $"{msgTypeName}.json");
            if (File.Exists(filename))
            {
                Logger.LogTrace("DeserializeMessage => Using schema file: {0}", filename);
                schemaJson = File.ReadAllText(filename);
            }
        }

        var reader = new JsonTextReader(new StringReader(msg.JsonPayload));
        var serializer = new JsonSerializer();

        if (!string.IsNullOrEmpty(schemaJson))
        {
            var validatingReader = new JSchemaValidatingReader(reader);
            validatingReader.Schema = JSchema.Parse(schemaJson);

            var messages = new List<string>();
            validatingReader.ValidationEventHandler += (o, a) => messages.Add(a.Message);
            var obj = serializer.Deserialize<T>(validatingReader);

            if (messages.Count > 0)
            {
                foreach (var err in messages)
                {
                    Logger.LogWarning("DeserializeMessage {0} => Validation error: {1}", msg.Action, err);
                }

                throw new FormatException("Message validation failed");
            }

            return obj;
        }

        // Deserialization WITHOUT schema validation
        Logger.LogTrace("DeserializeMessage => Deserialization without schema validation");
        return serializer.Deserialize<T>(reader);
    }
}