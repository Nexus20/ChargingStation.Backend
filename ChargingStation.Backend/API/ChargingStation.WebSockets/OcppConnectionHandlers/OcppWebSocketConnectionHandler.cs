using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.RegularExpressions;
using ChargingStation.Common.Constants;
using ChargingStation.Common.Messages_OCPP16.Requests;
using ChargingStation.Common.Models;
using ChargingStation.Common.Models.General;
using ChargingStation.WebSockets.Models;
using ChargingStation.WebSockets.OcppMessageHandlers.Providers;
using ChargingStation.WebSockets.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

namespace ChargingStation.WebSockets.OcppConnectionHandlers;

public class OcppWebSocketConnectionHandler : IOcppWebSocketConnectionHandler
{
    private static readonly string[] SupportedProtocols = [
        OcppProtocolVersions.Ocpp20,
        OcppProtocolVersions.Ocpp16
        /*, "ocpp1.5" */
    ];

    private const string MessageRegExp = "^\\[\\s*(\\d)\\s*,\\s*\"([^\"]*)\"\\s*,(?:\\s*\"(\\w*)\"\\s*,)?\\s*(.*)\\s*\\]$";

    private readonly ILogger<OcppWebSocketConnectionHandler> _logger;
    private readonly IChargePointCommunicationService _chargePointCommunicationService;
    private static ConcurrentDictionary<Guid, ChargePointInfo> _activeChargePoint = new ();

    private readonly IOcppMessageHandlerProvider _ocppMessageHandlerProvider;
    private string _subProtocol;

    public OcppWebSocketConnectionHandler(ILogger<OcppWebSocketConnectionHandler> logger, IChargePointCommunicationService chargePointCommunicationService, IOcppMessageHandlerProvider ocppMessageHandlerProvider)
    {
        _logger = logger;
        _chargePointCommunicationService = chargePointCommunicationService;
        _ocppMessageHandlerProvider = ocppMessageHandlerProvider;
    }

    public async Task HandleConnectionAsync(RequestDelegate next, HttpContext context)
    {
        var requestPath = context.Request.Path.Value;
        var requestParts = requestPath.Split('/');
        var chargePointId = Guid.Parse(requestParts[^1]);
        
        await _chargePointCommunicationService.CheckChargePointPresenceAsync(chargePointId);
        var protocolValidationResult = CheckProtocol(context);
        
        if (protocolValidationResult != null)
        {
            _logger.LogWarning("Protocol validation failed for charge point {ChargePointId}: {ProtocolValidationResult}", chargePointId, protocolValidationResult);
            await CloseInvalidSocketAsync(context, protocolValidationResult);
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            return;
        }
        
        using var socket = await context.WebSockets.AcceptWebSocketAsync();
        
        if (socket is null || socket.State != WebSocketState.Open)
        {
            await next(context);
            return;
        }
        
        if (!_activeChargePoint.ContainsKey(chargePointId))
        {
            _activeChargePoint.TryAdd(chargePointId, new ChargePointInfo(chargePointId, socket));
            _logger.LogInformation("No. of active chargers: {ChargersCount}", _activeChargePoint.Count);
        }
        else
        {
            try
            {
                var oldSocket = _activeChargePoint[chargePointId].WebSocket;
                _activeChargePoint[chargePointId].WebSocket = socket;
                
                if (oldSocket != null)
                {
                    _logger.LogInformation("New websocket request received for {ChargePointId}", chargePointId);
                    
                    if (oldSocket != socket && oldSocket.State != WebSocketState.Closed)
                    {
                        _logger.LogInformation("Closing old websocket for {ChargePointId}", chargePointId);

                        await oldSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client sent new websocket request", CancellationToken.None);
                    }
                }
                _logger.LogInformation("Websocket replaced successfully for {ChargePointId}", chargePointId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            
            if (socket.State == WebSocketState.Open)
            {
                await HandleActiveConnection(socket, chargePointId);
            }
        }
    }

    public async Task SendResponseAsync(Guid chargePointId, OcppMessage messageOut)
    {
        if (_activeChargePoint.TryGetValue(chargePointId, out var chargePointInfo))
        {
            var socket = chargePointInfo.WebSocket;
            
            if (socket.State == WebSocketState.Open)
            {
                string ocppTextResponse;

                if (string.IsNullOrEmpty(messageOut.ErrorCode))
                {

                    if (messageOut.MessageType == OcppMessageTypes.Call)
                    {
                        // OCPP-Request
                        ocppTextResponse = string.Format("[{0},\"{1}\",\"{2}\",{3}]", messageOut.MessageType,
                            messageOut.UniqueId, messageOut.Action, messageOut.JsonPayload);
                    }
                    else
                    {
                        // OCPP-Response
                        ocppTextResponse = string.Format("[{0},\"{1}\",{2}]", messageOut.MessageType,
                            messageOut.UniqueId, messageOut.JsonPayload);
                    }
                }
                else
                {
                    // OCPP-Error
                    ocppTextResponse = string.Format("[{0},\"{1}\",\"{2}\",\"{3}\",{4}]", messageOut.MessageType, messageOut.UniqueId, messageOut.ErrorCode, messageOut.ErrorDescription, "{}");
                }

                
                var responseBytes = Encoding.UTF8.GetBytes(ocppTextResponse);
                var responseSegment = new ArraySegment<byte>(responseBytes, 0, responseBytes.Length);
                await socket.SendAsync(responseSegment, WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }
    }

    public async Task SendResetAsync(Guid chargePointId, ResetRequest incomingRequest)
    {
        if (_activeChargePoint.TryGetValue(chargePointId, out var chargePointInfo))
        {
            var socket = chargePointInfo.WebSocket;
            
            if (socket.State == WebSocketState.Open)
            {
                var resetRequest = new OcppMessage(OcppMessageTypes.Call, Guid.NewGuid().ToString(), Ocpp16ActionTypes.Reset, JsonConvert.SerializeObject(incomingRequest));
                await SendResponseAsync(chargePointId, resetRequest);
                
                chargePointInfo.RequestDictionary.TryAdd(resetRequest.UniqueId, resetRequest);
            }
        }
    }

    public async Task SendCentralSystemRequestAsync(Guid chargePointId, OcppMessage centralSystemRequest, CancellationToken cancellationToken = default)
    {
        if (_activeChargePoint.TryGetValue(chargePointId, out var chargePointInfo))
        {
            var socket = chargePointInfo.WebSocket;
            
            if (socket.State == WebSocketState.Open)
            {
                await SendResponseAsync(chargePointId, centralSystemRequest);
                
                chargePointInfo.RequestDictionary.TryAdd(centralSystemRequest.UniqueId, centralSystemRequest);
            }
        }
    }

    private async Task HandleActiveConnection(WebSocket socket, Guid chargePointId)
    {
        if (socket.State == WebSocketState.Open)
            await HandlePayloadAsync(chargePointId, socket);

        if (socket.State != WebSocketState.Open 
            && _activeChargePoint.TryGetValue(chargePointId, out var chargePointInfo) 
            && chargePointInfo.WebSocket == socket)
            await RemoveConnectionsAsync(chargePointId, socket);
    }

    private async Task HandlePayloadAsync(Guid chargePointId, WebSocket webSocket)
    {
        while (webSocket.State == WebSocketState.Open)
        {
            try
            {
                var payloadString = await ReceiveDataFromChargerAsync(webSocket, chargePointId);
                
                if (payloadString == null)
                    return;
                
                await ProcessPayloadAsync(payloadString, chargePointId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }
    }

    private async Task ProcessPayloadAsync(string payloadString, Guid chargePointId)
    {
        var match = Regex.Match(payloadString, MessageRegExp);

        if (match?.Groups is { Count: >= 3 })
        {
            var messageTypeId = match.Groups[1].Value;
            var uniqueId = match.Groups[2].Value;
            var action = match.Groups[3].Value;
            var jsonPayload = match.Groups[4].Value;
            _logger.LogInformation("OCPPMiddleware.Receive16 => OCPP-Message: Type={MessageTypeId} / ID={UniqueId} / Action={Action})", messageTypeId, uniqueId, action);
            
            var incomingMessage = new OcppMessage(messageTypeId, uniqueId, action, jsonPayload);
            
            // var validationResponse = ValidateSchema(JObject.Parse(jsonPayload), action);
            //
            // if (!validationResponse.Valid)
            // {
            //     throw new NotImplementedException();
            // }

            switch (incomingMessage.MessageType)
            {
                case OcppMessageTypes.Call:
                    // Process Request
                    var messageHandler = _ocppMessageHandlerProvider.GetRequestHandler(action, _subProtocol);
                    await messageHandler.HandleAsync(incomingMessage, chargePointId);
                    break;
                case OcppMessageTypes.CallResult:
                    
                    if (_activeChargePoint.TryGetValue(chargePointId, out var chargePointInfo))
                    {
                        if (chargePointInfo.RequestDictionary.Remove(uniqueId, out var request))
                        {
                            var responseHandler = _ocppMessageHandlerProvider.GetResponseHandler(request.Action, _subProtocol);
                            await responseHandler.HandleAsync(incomingMessage, chargePointId);
                        }
                        else
                        {
                            _logger.LogWarning("No active request with id {RequestId} found for charge point {ChargePointId}", uniqueId, chargePointId);
                        }
                    }
                    else
                    {
                        _logger.LogWarning("No active charge point found for {ChargePointId}", chargePointId);
                    }
                    
                    break;
                case OcppMessageTypes.CallError:
                    // Process Error
                    break;
                default:
                    throw new InvalidOperationException("Invalid message type");
            }
        }
    }

    private JsonValidationResponse ValidateSchema(JObject payload, string action)
    {
        var response = new JsonValidationResponse { Valid = false };

        try
        {
            //Getting Schema FilePath
            var currentDirectory = Directory.GetCurrentDirectory();
            var filePath = Path.Combine(currentDirectory, "Schemas", $"{action}.json");

            //Parsing schema
            var content = JObject.Parse(File.ReadAllText(filePath));
            var schema = JSchema.Parse(content.ToString());
            var json = JToken.Parse(payload.ToString()); // Parsing input payload

            // Validate json
            response = new JsonValidationResponse
            {
                Valid = json.IsValid(schema, out IList<ValidationError> errors),
                Errors = errors.ToList()
            };
        }
        catch (FileNotFoundException)
        {
            response.CustomErrors = "Schema file not found";
        }
        catch (JsonReaderException jsre)
        {
            _logger.LogError(jsre, "Error while parsing JSON");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
        }

        return response;
    }

    private async Task<string?> ReceiveDataFromChargerAsync(WebSocket webSocket, Guid chargePointId)
    {
        try
        {
            var data = new ArraySegment<byte>(new byte[1024]);
            WebSocketReceiveResult result;
            var payloadStringBuilder = new StringBuilder();

            do
            {
                result = await webSocket.ReceiveAsync(data, CancellationToken.None);

                //When the charger sends close frame
                if (result.CloseStatus.HasValue)
                {
                    if (webSocket != _activeChargePoint[chargePointId].WebSocket)
                    {
                        if(webSocket.State != WebSocketState.CloseReceived)
                            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "New websocket request received for this charger", CancellationToken.None);
                    }
                    else
                        await RemoveConnectionsAsync(chargePointId, webSocket);
                    
                    return null;
                }

                //Appending received data
                var payloadPart = Encoding.UTF8.GetString(data.Array, 0, result.Count);
                payloadStringBuilder.Append(payloadPart);

            } while (!result.EndOfMessage);

            return payloadStringBuilder.ToString();
        }
        catch (WebSocketException webSocketException)
        {
            if (webSocket != _activeChargePoint[chargePointId].WebSocket)
                _logger.LogError("WebsocketException occured in the old socket while receiving payload from charger {ChargePointId}. Error: {ExceptionMessage}", chargePointId, webSocketException.Message);
            else
                _logger.LogError(webSocketException, webSocketException.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
        }

        return null;
    }
    
    private async Task CloseInvalidSocketAsync(HttpContext context, string protocolValidationResult)
    {
        using var socket = await context.WebSockets.AcceptWebSocketAsync();
        await socket.CloseOutputAsync(WebSocketCloseStatus.ProtocolError, protocolValidationResult, CancellationToken.None);
    }

    private string? CheckProtocol(HttpContext httpContext)
    {
        var ocppProtocols = httpContext.WebSockets.WebSocketRequestedProtocols;
        if (ocppProtocols.Count == 0)
            return "No sub-protocol header";

        if(!SupportedProtocols.Any(protocol => ocppProtocols.Contains(protocol)))
            return "Sub-protocol not supported";
        
        foreach (var supportedProtocol in SupportedProtocols)
        {
            if (ocppProtocols.Contains(supportedProtocol))
            {
                _subProtocol = supportedProtocol;
                break;
            }
        }
        
        return null;
    }
    
    private async Task RemoveConnectionsAsync(Guid chargePointId, WebSocket webSocket)
    {
        try
        {
            if (_activeChargePoint.TryRemove(chargePointId, out _))
            {
                _logger.LogInformation("Removed charger {ChargePointId}", chargePointId);
            }
            else
            {
                _logger.LogWarning("Cannot remove charger {ChargePointId}", chargePointId);
            }

            if (webSocket.State == WebSocketState.Aborted)
                await webSocket.CloseOutputAsync(WebSocketCloseStatus.ProtocolError, "Protocol error", CancellationToken.None);
            else
                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client requested closure", CancellationToken.None);

            _logger.LogInformation("Closed websocket for charger {ChargePointId}. Remaining active chargers: {ActiveChargePointsCount}", chargePointId, _activeChargePoint.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
        }
    }
}