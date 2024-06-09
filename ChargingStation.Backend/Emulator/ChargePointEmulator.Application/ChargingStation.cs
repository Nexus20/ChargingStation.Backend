using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.RegularExpressions;
using ChargePointEmulator.Application.Interfaces;
using ChargePointEmulator.Application.State;
using ChargingStation.Common.Constants;
using ChargingStation.Common.Messages_OCPP16;
using ChargingStation.Common.Messages_OCPP16.Enums;
using ChargingStation.Common.Messages_OCPP16.Requests;
using ChargingStation.Common.Messages_OCPP16.Requests.Enums;
using ChargingStation.Common.Messages_OCPP16.Responses;
using ChargingStation.Common.Messages_OCPP16.Responses.Enums;
using ChargingStation.Common.Models.General;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;

namespace ChargePointEmulator.Application;

public class ChargingStation : IAsyncDisposable
{
    private readonly IChargingStationStateRepository _stateRepository;
    private const string MessageRegExp = "^\\[\\s*(\\d)\\s*,\\s*\"([^\"]*)\"\\s*,(?:\\s*\"(\\w*)\"\\s*,)?\\s*(.*)\\s*\\]$";
    private const string OcppProtocol = "ocpp1.6";
    private readonly SemaphoreSlim _connectSemaphore = new(1, 1);
    private readonly SemaphoreSlim _stateSemaphore = new(1, 1);
    
    private ClientWebSocket?  _webSocket;
    public WebSocketState WebSocketState => _webSocket?.State ?? WebSocketState.None;
    private string _endPoint;

    public Guid Id { get; }

    private readonly HubConnection? _hubConnection;

    public ChargingStationState State { get; set; }

    public ChargingStation(Guid id, IChargingStationStateRepository stateRepository, string? hubUri = null)
    {
        Id = id;
        _stateRepository = stateRepository;
        State = new ChargingStationState { Id = id.ToString() };
        
        if (hubUri is not null)
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(hubUri)
                .Build();
        }
    }

    public async Task<string> SendAuthorizeRequestAsync(string idTag, CancellationToken cancellationToken = default)
    {
        try
        {
            var authorizeRequest = new AuthorizeRequest(idTag);
            var jsonPayload = JsonConvert.SerializeObject(authorizeRequest);
            var messageId = Guid.NewGuid().ToString();
            var ocppMessage = new OcppMessage(OcppMessageTypes.Call, messageId, Ocpp16ActionTypes.Authorize, jsonPayload);
            var textMessage = $"[{ocppMessage.MessageType},\"{ocppMessage.UniqueId}\",\"{ocppMessage.Action}\",{ocppMessage.JsonPayload}]";
            
            await SendMessageAsync(textMessage, cancellationToken);
            State.PendingRequests.TryAdd(messageId, ocppMessage);
            await SaveStateAsync(cancellationToken);

            return messageId;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error during sending authorize message: {e.Message}");
            throw;
        }
    }
    
    public async Task SendStartTransactionRequestAsync(int connectorId, string idTag, int? reservationId = null, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!State.AuthorizedOcppTags.TryGetValue(idTag, out var idTagInfo) || (idTagInfo.ExpiryDate is not null && idTagInfo.ExpiryDate < DateTimeOffset.UtcNow))
            {
                var authCompletionSource = new TaskCompletionSource<AuthorizeResponse>(TaskCreationOptions.RunContinuationsAsynchronously);
                var authMessageId = await SendAuthorizeRequestAsync(idTag, cancellationToken);
                State.PendingAuthorizeRequests.TryAdd(authMessageId, authCompletionSource);
                await SaveStateAsync(cancellationToken);
                
                var authorizeResponse = await authCompletionSource.Task;
                
                if (authorizeResponse.IdTagInfo.Status != IdTagInfoStatus.Accepted)
                {
                    throw new Exception("Authorization failed for idTag " + idTag);
                }

                State.PendingAuthorizeRequests.TryRemove(authMessageId, out var _);
            };
            
            var startTransactionRequest = new StartTransactionRequest(connectorId, idTag, 0, DateTime.UtcNow)
            {
                ReservationId = reservationId
            };
            
            var jsonPayload = JsonConvert.SerializeObject(startTransactionRequest);
            var messageId = Guid.NewGuid().ToString();
            var ocppMessage = new OcppMessage(OcppMessageTypes.Call, messageId, Ocpp16ActionTypes.StartTransaction, jsonPayload);
            var textMessage = $"[{ocppMessage.MessageType},\"{ocppMessage.UniqueId}\",\"{ocppMessage.Action}\",{ocppMessage.JsonPayload}]";
            
            await SendMessageAsync(textMessage, cancellationToken);
            State.PendingRequests.TryAdd(messageId, ocppMessage);
            if (!State.Connectors.TryAdd(connectorId, new ConnectorState
                {
                    ConnectorId = connectorId,
                    Status = StatusNotificationRequestStatus.Charging
                }))
            {
                State.Connectors[connectorId].Status = StatusNotificationRequestStatus.Charging;
            };
            await SaveStateAsync(cancellationToken);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error during sending start transaction message: {e.Message}");
            throw;
        }
    }
    
    public async Task SendMeterValuesRequestAsync(int connectorId, int transactionId, CancellationToken cancellationToken = default)
    {
        try
        {
            var mockRequests = GenerateMeterValuesForChargingSession(connectorId, transactionId, 10, 10);
            
            foreach (var request in mockRequests)
            {
                await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);
                
                var jsonPayload = JsonConvert.SerializeObject(request);
                var messageId = Guid.NewGuid().ToString();
                var ocppMessage = new OcppMessage(OcppMessageTypes.Call, messageId, Ocpp16ActionTypes.MeterValues, jsonPayload);
                var textMessage = $"[{ocppMessage.MessageType},\"{ocppMessage.UniqueId}\",\"{ocppMessage.Action}\",{ocppMessage.JsonPayload}]";
                
                await SendMessageAsync(textMessage, cancellationToken);
                State.Connectors[connectorId].LastTransaction?.MeterValues.Add(request);
                State.PendingRequests.TryAdd(messageId, ocppMessage);
                await SaveStateAsync(cancellationToken);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error during sending meter values message: {e.Message}");
            throw;
        }
    }

    private List<MeterValuesRequest> GenerateMeterValuesForChargingSession(int connectorId, int transactionId, int intervalInSeconds, int totalIntervals)
    {
        var meterValuesRequests = new List<MeterValuesRequest>();
        double energyConsumed = 0; // Start from 0 кВт*ч
        double power = 50; // Power W
        var random = new Random();

        for (var i = 0; i < totalIntervals; i++)
        {
            energyConsumed += power * (intervalInSeconds / 3600.0); // Add energy consumed in this interval

            var meterValue = new MeterValue(
                Timestamp: DateTimeOffset.UtcNow.AddSeconds(i * intervalInSeconds),
                SampledValue: new List<SampledValue>
                {
                    new(Value: energyConsumed.ToString("F2"))
                    {
                        Measurand = SampledValueMeasurand.Energy_Active_Import_Register,
                        Format = SampledValueFormat.Raw,
                        Unit = SampledValueUnit.Wh
                    },
                    new(Value: power.ToString("F2"))
                    {
                        Measurand = SampledValueMeasurand.Power_Active_Import,
                        Format = SampledValueFormat.Raw,
                        Unit = SampledValueUnit.W
                    },
                    new(Value: random.Next(220, 240).ToString()) // Пример изменения напряжения
                    {
                        Measurand = SampledValueMeasurand.Voltage,
                        Format = SampledValueFormat.Raw,
                        Unit = SampledValueUnit.V
                    },
                    new(Value: random.Next(16, 32).ToString()) // Пример изменения тока
                    {
                        Measurand = SampledValueMeasurand.Current_Import,
                        Format = SampledValueFormat.Raw,
                        Unit = SampledValueUnit.A
                    },
                    new(Value: (100 / totalIntervals * (i + 1)).ToString()) // Пример изменения SoC
                    {
                        Measurand = SampledValueMeasurand.SoC,
                        Format = SampledValueFormat.Raw,
                        Unit = SampledValueUnit.Percent
                    }
                }
            );

            meterValuesRequests.Add(new MeterValuesRequest(connectorId, new[] { meterValue })
                { TransactionId = transactionId });
        }

        return meterValuesRequests;
    }

    
    public async Task SendStopTransactionRequestAsync(int transactionId, string idTag,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var stopTransactionRequest = new StopTransactionRequest(0, DateTime.UtcNow, transactionId) {
                IdTag = idTag
            };
            var jsonPayload = JsonConvert.SerializeObject(stopTransactionRequest);
            var messageId = Guid.NewGuid().ToString();
            var ocppMessage = new OcppMessage(OcppMessageTypes.Call, messageId, Ocpp16ActionTypes.StopTransaction, jsonPayload);
            var textMessage = $"[{ocppMessage.MessageType},\"{ocppMessage.UniqueId}\",\"{ocppMessage.Action}\",{ocppMessage.JsonPayload}]";
            
            await SendMessageAsync(textMessage, cancellationToken);
            State.PendingRequests.TryAdd(messageId, ocppMessage);
            var connectorId = State.Connectors.First(x => x.Value.LastTransaction?.TransactionId == transactionId).Key;
            State.Connectors[connectorId].Status = StatusNotificationRequestStatus.Available;
            State.Connectors[connectorId].LastTransaction.StopTimestamp = DateTime.UtcNow;
            await SaveStateAsync(cancellationToken);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error during sending stop transaction message: {e.Message}");
            throw;
        }
    }
    
    public async Task SendBootNotificationRequestAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var bootNotificationMessage = new BootNotificationRequest("AVT-Company", "AVT-Express")
            {
                ChargePointSerialNumber = "avt.001.13.1",
                ChargeBoxSerialNumber = "avt.001.13.1.01",
                FirmwareVersion = "0.9.87",
                MeterType = "AVT NQC-ACDC",
                MeterSerialNumber = "avt.001.13.1.01"
            };
            var jsonPayload = JsonConvert.SerializeObject(bootNotificationMessage);
            var messageId = Guid.NewGuid().ToString();
            var ocppMessage = new OcppMessage(OcppMessageTypes.Call, messageId, Ocpp16ActionTypes.BootNotification, jsonPayload);
            var textMessage = string.Format("[{0},\"{1}\",\"{2}\",{3}]", ocppMessage.MessageType, ocppMessage.UniqueId, ocppMessage.Action, ocppMessage.JsonPayload);
            
            await SendMessageAsync(textMessage, cancellationToken);
            State.PendingRequests.TryAdd(messageId, ocppMessage);
            await SaveStateAsync(cancellationToken);
            Console.WriteLine("Boot notification message sent.");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error during sending boot notification message: {e.Message}");
            throw;
        }
    }
    
    public async Task InitializeAndStartAsync(string endpoint, CancellationToken cancellationToken = default)
    {
        try
        {
            var state = await _stateRepository.GetByIdAsync(Id.ToString(), cancellationToken);
            
            if (state != null)
                State = state;
            else
                await _stateRepository.InsertAsync(State, cancellationToken);
            
            await _connectSemaphore.WaitAsync(cancellationToken);
            try
            {
                await ConnectAsync(endpoint, cancellationToken);
            }
            finally
            {
                _connectSemaphore.Release();
            }
            
            _ = Task.Run(async () =>
            {
                try
                {
                    await StartReceivingMessagesAsync(cancellationToken);
                }
                catch (WebSocketException ex)
                {
                    Console.WriteLine("WebSocket connection closed prematurely. Attempting to reconnect...");
                    await _connectSemaphore.WaitAsync(cancellationToken);
                    try
                    {
                        await ReconnectAsync(endpoint, cancellationToken);
                    }
                    finally
                    {
                        _connectSemaphore.Release();
                    }
                    await StartReceivingMessagesAsync(cancellationToken);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error in receiving messages: {e.Message}");
                }
            }, cancellationToken);
            
            await Task.Delay(TimeSpan.FromSeconds(3), cancellationToken);
            
            await _connectSemaphore.WaitAsync(cancellationToken);
            try
            {
                await SendBootNotificationRequestAsync(cancellationToken);
            }
            finally
            {
                _connectSemaphore.Release();
            }
            
            if(_hubConnection is not null && _hubConnection.State == HubConnectionState.Disconnected)
            {
                await _hubConnection.StartAsync(cancellationToken);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error during initialization and start: {e.Message}");
            throw;
        }
    }
    
    public async Task ReconnectAsync(string endpoint, CancellationToken cancellationToken)
    {
        const int maxReconnectAttempts = 5; // Максимальное количество попыток реконнекта
        var reconnectAttempts = 0;

        while (reconnectAttempts < maxReconnectAttempts && !cancellationToken.IsCancellationRequested)
        {
            try
            {
                if (_webSocket.State == WebSocketState.Open)
                {
                    // Если соединение уже открыто, нет необходимости в дальнейших попытках
                    break;
                }

                if (_webSocket.State != WebSocketState.Closed)
                {
                    _webSocket.Abort();
                }

                _webSocket.Dispose();
                _webSocket = new ClientWebSocket();
                _webSocket.Options.AddSubProtocol(OcppProtocol);

                await _webSocket.ConnectAsync(new Uri($"{endpoint}/OCPP/{Id}"), cancellationToken);

                if (_webSocket.State == WebSocketState.Open)
                {
                    // Соединение успешно установлено
                    Console.WriteLine("WebSocket reconnected successfully.");
                    break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Attempt {reconnectAttempts + 1} failed to reconnect: {ex.Message}");
            }

            reconnectAttempts++;
            await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken); // Ожидание перед следующей попыткой
        }

        if (reconnectAttempts >= maxReconnectAttempts)
            throw new Exception("Failed to reconnect after maximum attempts.");
    }


    public async Task ConnectAsync(string endpoint, CancellationToken cancellationToken = default)
    {
        try
        {
            _webSocket = new ClientWebSocket();
            _endPoint = endpoint;
            var uri = new Uri($"{endpoint}/OCPP/{Id}");
            _webSocket.Options.AddSubProtocol(OcppProtocol);
            _webSocket.Options.KeepAliveInterval = TimeSpan.FromSeconds(30);
            await _webSocket.ConnectAsync(uri, cancellationToken);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    public async Task DisconnectAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            if (_webSocket.State is WebSocketState.Open or WebSocketState.Connecting)
            {
                await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Charging station disconnected", cancellationToken);
                
                if (_hubConnection is not null && _hubConnection.State == HubConnectionState.Connected)
                {
                    await _hubConnection.StopAsync(cancellationToken);
                }
                
                _webSocket.Dispose();
            }
            
            await SaveStateAsync(cancellationToken);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error during stopping the station: {e.Message}");
            throw;
        }
    }

    public async Task SaveStateAsync(CancellationToken cancellationToken = default)
    {
        await _stateSemaphore.WaitAsync(cancellationToken);
        try
        {
            // Ваши действия по сохранению состояния
            await _stateRepository.UpdateAsync(State, cancellationToken);
        }
        finally
        {
            _stateSemaphore.Release();
        }
    }

    public async Task StartReceivingMessagesAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested && _webSocket.State == WebSocketState.Open)
        {
            try
            {
                var payloadString = await ReceiveMessageAsync(cancellationToken);
            
                if (payloadString == null)
                    continue;
            
                // Handle received message
                await HandleMessageAsync(payloadString, cancellationToken);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        
        if(cancellationToken.IsCancellationRequested)
            Console.WriteLine("Cancellation requested. Stopped receiving messages.");
    }

    public async Task SendMessageAsync(string jsonPayload, CancellationToken cancellationToken)
    {
        try
        {
            if (_webSocket.State != WebSocketState.Open)
            {
                    throw new InvalidOperationException("WebSocket is not open");
            }

            var payload = Encoding.UTF8.GetBytes(jsonPayload);
            var segment = new ArraySegment<byte>(payload);
            await _webSocket.SendAsync(segment, WebSocketMessageType.Text, true, cancellationToken);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error during sending message: {e.Message}");
            throw;
        }
    }

    private async Task<string?> ReceiveMessageAsync(CancellationToken cancellationToken)
    {
        var data = new ArraySegment<byte>(new byte[1024]);
        WebSocketReceiveResult result;
        var payloadStringBuilder = new StringBuilder();

        do
        {
            result = await _webSocket.ReceiveAsync(data, cancellationToken);

            //When the charger sends close frame
            if (result.CloseStatus.HasValue)
            {
                if(_webSocket.State != WebSocketState.CloseReceived)
                    await _webSocket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "New websocket request received for this charger", CancellationToken.None);
                    
                return null;
            }

            //Appending received data
            var payloadPart = Encoding.UTF8.GetString(data.Array, 0, result.Count);
            payloadStringBuilder.Append(payloadPart);

        } while (!result.EndOfMessage);

        return payloadStringBuilder.ToString();
    }
    
    private async Task HandleMessageAsync(string payloadString, CancellationToken cancellationToken = default)
    {
        var match = Regex.Match(payloadString, MessageRegExp);

        if (match?.Groups is { Count: >= 3 })
        {
            var messageTypeId = match.Groups[1].Value;
            var uniqueId = match.Groups[2].Value;
            var action = match.Groups[3].Value;
            var jsonPayload = match.Groups[4].Value;
            
            Console.WriteLine($"Station {Id} received message: {messageTypeId}, {uniqueId}, {action}, {jsonPayload}");
            
            switch (messageTypeId)
            {
                case OcppMessageTypes.Call:
                    await HandleCentralSystemRequestAsync(uniqueId, action, jsonPayload, cancellationToken);
                    break;
                
                case OcppMessageTypes.CallResult:
                    await HandleCentralSystemResponseAsync(uniqueId, jsonPayload, cancellationToken);
                    break;
                
                case OcppMessageTypes.CallError:
                    break;
                
                default:
                    throw new InvalidOperationException("Unknown message type");
            }
            
            State.LastMessageReceived = DateTime.UtcNow;
            await SaveStateAsync(cancellationToken);
            await SendUpdateToHubAsync(cancellationToken);
        }
    }

    private async Task HandleCentralSystemRequestAsync(string uniqueId, string action, string jsonPayload, CancellationToken cancellationToken)
    {
        switch (action)
        {
            case Ocpp16ActionTypes.ReserveNow:
                var reserveNowRequest = JsonConvert.DeserializeObject<ReserveNowRequest>(jsonPayload);
                await HandleReserveNowRequestAsync(reserveNowRequest, uniqueId, cancellationToken);
                break;
            case Ocpp16ActionTypes.CancelReservation:
                var cancelReservationRequest = JsonConvert.DeserializeObject<CancelReservationRequest>(jsonPayload);
                await HandleCancelReservationRequestAsync(cancelReservationRequest, uniqueId, cancellationToken);
                break;
            case Ocpp16ActionTypes.ChangeAvailability:
                var changeAvailabilityRequest = JsonConvert.DeserializeObject<ChangeAvailabilityRequest>(jsonPayload);
                await HandleChangeAvailabilityRequestAsync(changeAvailabilityRequest, uniqueId, cancellationToken);
                break;
            case Ocpp16ActionTypes.SetChargingProfile:
                var chargingProfileRequest = JsonConvert.DeserializeObject<SetChargingProfileRequest>(jsonPayload);
                await HandleSetChargingProfileRequestAsync(chargingProfileRequest, uniqueId, cancellationToken);
                break;
            case Ocpp16ActionTypes.ClearChargingProfile:
                var clearChargingProfileRequest = JsonConvert.DeserializeObject<ClearChargingProfileRequest>(jsonPayload);
                await HandleClearChargingProfileRequestAsync(clearChargingProfileRequest, uniqueId, cancellationToken);
                break;
        }
    }

    private async Task HandleClearChargingProfileRequestAsync(ClearChargingProfileRequest request, string uniqueId, CancellationToken cancellationToken)
    {
        var profileRemoved = false;

        if (request.Id.HasValue)
        {
            var connectorId = State.Connectors.Values.FirstOrDefault(x => x.ChargingProfiles.ContainsKey(request.Id.Value))?.ConnectorId;

            if (connectorId.HasValue)
            {
                State.Connectors[connectorId.Value].ChargingProfiles.Remove(request.Id.Value, out _);
                await SaveStateAsync(cancellationToken);
                profileRemoved = true;
            }
        } 
        else if (request.ConnectorId.HasValue)
        {
            if (State.Connectors.TryGetValue(request.ConnectorId.Value, out _))
            {
                State.Connectors[request.ConnectorId.Value].ChargingProfiles.Clear();
                await SaveStateAsync(cancellationToken);
                profileRemoved = true;
            }
        }
        else if (request.ChargingProfilePurpose.HasValue)
        {
            foreach (var connector in State.Connectors.Values)
            {
                var profilesToRemove = connector.ChargingProfiles.Values.Where(x => (int)x.ChargingProfilePurpose == (int)request.ChargingProfilePurpose.Value).ToList();
                
                if(profilesToRemove.Count == 0)
                    continue;
                
                foreach (var profile in profilesToRemove)
                {
                    connector.ChargingProfiles.Remove(profile.ChargingProfileId, out _);
                    profileRemoved = true;
                }
            }
            
            if(profileRemoved)
                await SaveStateAsync(cancellationToken);
        } else if (request.StackLevel.HasValue)
        {
            foreach (var connector in State.Connectors.Values)
            {
                var profilesToRemove = connector.ChargingProfiles.Values.Where(x => x.StackLevel == request.StackLevel.Value).ToList();
                
                if(profilesToRemove.Count == 0)
                    continue;
                
                foreach (var profile in profilesToRemove)
                {
                    connector.ChargingProfiles.Remove(profile.ChargingProfileId, out _);
                    profileRemoved = true;
                }
            }
            
            if(profileRemoved)
                await SaveStateAsync(cancellationToken);
        }
        
        var response = new ClearChargingProfileResponse(profileRemoved ? ClearChargingProfileResponseStatus.Accepted : ClearChargingProfileResponseStatus.Unknown);
        var jsonPayload = JsonConvert.SerializeObject(response);
        var ocppMessage = new OcppMessage(OcppMessageTypes.CallResult, uniqueId, jsonPayload);
        var textMessage = $"[{ocppMessage.MessageType},\"{ocppMessage.UniqueId}\",{ocppMessage.JsonPayload}]";
        await SendMessageAsync(textMessage, cancellationToken);
    }

    private async Task HandleSetChargingProfileRequestAsync(SetChargingProfileRequest request, string uniqueId, CancellationToken cancellationToken)
    {
        if(!State.Connectors.TryGetValue(request.ConnectorId, out var connector))
            throw new InvalidOperationException("Connector not found");
        
        if (request.CsChargingProfiles.ChargingProfilePurpose != CsChargingProfilesChargingProfilePurpose.TxProfile)
        {
            // find charging profile with same profile id
            if(connector.ChargingProfiles.TryGetValue(request.CsChargingProfiles.ChargingProfileId, out var profile))
            {
                connector.ChargingProfiles[request.CsChargingProfiles.ChargingProfileId] = request.CsChargingProfiles;
            }
            else
            {
                var sameStackAndPurposeProfile = connector.ChargingProfiles.Values.FirstOrDefault(x => x.StackLevel == request.CsChargingProfiles.StackLevel && x.ChargingProfilePurpose == request.CsChargingProfiles.ChargingProfilePurpose);
                
                if (sameStackAndPurposeProfile is not null)
                {
                    connector.ChargingProfiles[sameStackAndPurposeProfile.ChargingProfileId] = request.CsChargingProfiles;
                }
                else
                {
                    connector.ChargingProfiles.TryAdd(request.CsChargingProfiles.ChargingProfileId, request.CsChargingProfiles);
                }
            }

            await SaveStateAsync(cancellationToken);
            var response = new SetChargingProfileResponse(SetChargingProfileResponseStatus.Accepted);
            var jsonPayload = JsonConvert.SerializeObject(response);
            var ocppMessage = new OcppMessage(OcppMessageTypes.CallResult, uniqueId, jsonPayload);
            var textMessage = $"[{ocppMessage.MessageType},\"{ocppMessage.UniqueId}\",{ocppMessage.JsonPayload}]";
            await SendMessageAsync(textMessage, cancellationToken);
        }
        
        if (request.CsChargingProfiles.TransactionId.HasValue)
        {
            // handle charging profile for particular transaction
        }
        else
        {
            
        }
    }
    
    private async Task HandleReserveNowRequestAsync(ReserveNowRequest request, string uniqueId, CancellationToken cancellationToken)
    {
        if (State.Reservations.TryGetValue(request.ReservationId, out _))
        {
            State.Reservations[request.ReservationId].ConnectorId = request.ConnectorId;
            State.Reservations[request.ReservationId].ExpirationDate = request.ExpiryDate;
            
            var reserveNowResponse = new ReserveNowResponse(ReserveNowResponseStatus.Accepted);
            
            await SaveStateAsync(cancellationToken);
            var jsonPayload = JsonConvert.SerializeObject(reserveNowResponse);
            var ocppMessage = new OcppMessage(OcppMessageTypes.CallResult, uniqueId, jsonPayload);
            var textMessage = $"[{ocppMessage.MessageType},\"{ocppMessage.UniqueId}\",{ocppMessage.JsonPayload}]";
            await SendMessageAsync(textMessage, cancellationToken);
            return;
        }

        if (State.Connectors.TryGetValue(request.ConnectorId, out var connectorState))
        {

            if (connectorState.Status
                is StatusNotificationRequestStatus.Preparing
                or StatusNotificationRequestStatus.Charging
                or StatusNotificationRequestStatus.SuspendedEVSE
                or StatusNotificationRequestStatus.SuspendedEV
                or StatusNotificationRequestStatus.Finishing)
            {
                var reserveNowResponse = new ReserveNowResponse(ReserveNowResponseStatus.Occupied);
            
                var jsonPayload = JsonConvert.SerializeObject(reserveNowResponse);
                var ocppMessage = new OcppMessage(OcppMessageTypes.CallResult, uniqueId, jsonPayload);
                var textMessage = $"[{ocppMessage.MessageType},\"{ocppMessage.UniqueId}\",{ocppMessage.JsonPayload}]";
                await SendMessageAsync(textMessage, cancellationToken);
                return;
            }

            if (connectorState.Status == StatusNotificationRequestStatus.Faulted)
            {
                var reserveNowResponse = new ReserveNowResponse(ReserveNowResponseStatus.Faulted);
                
                var jsonPayload = JsonConvert.SerializeObject(reserveNowResponse);
                var ocppMessage = new OcppMessage(OcppMessageTypes.CallResult, uniqueId, jsonPayload);
                var textMessage = $"[{ocppMessage.MessageType},\"{ocppMessage.UniqueId}\",{ocppMessage.JsonPayload}]";
                await SendMessageAsync(textMessage, cancellationToken);
                return;
            }
            
            if (connectorState.Status == StatusNotificationRequestStatus.Unavailable)
            {
                var reserveNowResponse = new ReserveNowResponse(ReserveNowResponseStatus.Unavailable);
                
                var jsonPayload = JsonConvert.SerializeObject(reserveNowResponse);
                var ocppMessage = new OcppMessage(OcppMessageTypes.CallResult, uniqueId, jsonPayload);
                var textMessage = $"[{ocppMessage.MessageType},\"{ocppMessage.UniqueId}\",{ocppMessage.JsonPayload}]";
                await SendMessageAsync(textMessage, cancellationToken);
                return;
            }
        }
        
        if(State.Reservations.Values.Any(x => x.ConnectorId == request.ConnectorId && x.IdTag != request.IdTag))
        {
            var reserveNowResponse = new ReserveNowResponse(ReserveNowResponseStatus.Occupied);
            
            var jsonPayload = JsonConvert.SerializeObject(reserveNowResponse);
            var ocppMessage = new OcppMessage(OcppMessageTypes.CallResult, uniqueId, jsonPayload);
            var textMessage = $"[{ocppMessage.MessageType},\"{ocppMessage.UniqueId}\",{ocppMessage.JsonPayload}]";
            await SendMessageAsync(textMessage, cancellationToken);
            return;
        }
        
        {
            var reservationState = new ReservationState
            {
                ConnectorId = request.ConnectorId,
                ReservationId = request.ReservationId,
                Status = ReserveNowResponseStatus.Accepted,
                ExpirationDate = request.ExpiryDate,
                IdTag = request.IdTag
            };

            State.Reservations.TryAdd(request.ReservationId, reservationState);
            await SaveStateAsync(cancellationToken);

            var reserveNowResponse = new ReserveNowResponse(ReserveNowResponseStatus.Accepted);
            var jsonPayload = JsonConvert.SerializeObject(reserveNowResponse);
            var ocppMessage = new OcppMessage(OcppMessageTypes.CallResult, uniqueId, jsonPayload);
            var textMessage = $"[{ocppMessage.MessageType},\"{ocppMessage.UniqueId}\",{ocppMessage.JsonPayload}]";
            await SendMessageAsync(textMessage, cancellationToken);
        }
    }
    
    private async Task HandleCancelReservationRequestAsync(CancelReservationRequest request, string uniqueId, CancellationToken cancellationToken)
    {
        var cancelReservationResponse = State.Reservations.TryRemove(request.ReservationId, out _) 
            ? new CancelReservationResponse(CancelReservationResponseStatus.Accepted) 
            : new CancelReservationResponse(CancelReservationResponseStatus.Rejected);
        
        var jsonPayload = JsonConvert.SerializeObject(cancelReservationResponse);
        var ocppMessage = new OcppMessage(OcppMessageTypes.CallResult, uniqueId, jsonPayload);
        var textMessage = $"[{ocppMessage.MessageType},\"{ocppMessage.UniqueId}\",{ocppMessage.JsonPayload}]";
        await SendMessageAsync(textMessage, cancellationToken);
    }
    
    private async Task HandleChangeAvailabilityRequestAsync(ChangeAvailabilityRequest request, string uniqueId, CancellationToken cancellationToken)
    {
        ChangeAvailabilityResponse response;
        
        if (request.ConnectorId == 0)
        {
            var activeTransactionsPresent = State.Connectors.Values.Any(x => x.LastTransaction is not null && x.LastTransaction.StopTimestamp is null);

            // TODO: Add sending status notification for pending change availability
            response = activeTransactionsPresent ? new ChangeAvailabilityResponse(ChangeAvailabilityResponseStatus.Scheduled)
                                                 : new ChangeAvailabilityResponse(ChangeAvailabilityResponseStatus.Accepted);
        }
        else
        {
            var activeTransactionPresent = State.Connectors.TryGetValue(request.ConnectorId, out var connectorState) 
                                           && connectorState.LastTransaction is not null 
                                           && connectorState.LastTransaction.StopTimestamp is null;

            response = activeTransactionPresent ? new ChangeAvailabilityResponse(ChangeAvailabilityResponseStatus.Scheduled) 
                                                : new ChangeAvailabilityResponse(ChangeAvailabilityResponseStatus.Accepted);
        }
        
        var jsonPayload = JsonConvert.SerializeObject(response);
        var ocppMessage = new OcppMessage(OcppMessageTypes.CallResult, uniqueId, jsonPayload);
        var textMessage = $"[{ocppMessage.MessageType},\"{ocppMessage.UniqueId}\",{ocppMessage.JsonPayload}]";
        await SendMessageAsync(textMessage, cancellationToken);

        if (response.Status == ChangeAvailabilityResponseStatus.Accepted)
        {
            if (request.ConnectorId == 0)
            {
                foreach (var (_, connectorState) in State.Connectors)
                {
                    connectorState.Status = request.Type == ChangeAvailabilityRequestType.Operative
                        ? StatusNotificationRequestStatus.Available
                        : StatusNotificationRequestStatus.Unavailable;
                    
                    await SendConnectorStatusNotificationAsync(connectorState.ConnectorId, connectorState.Status, cancellationToken);
                }
            }
            else
            {
                State.Connectors[request.ConnectorId].Status = request.Type == ChangeAvailabilityRequestType.Operative
                    ? StatusNotificationRequestStatus.Available
                    : StatusNotificationRequestStatus.Unavailable;
                await SendConnectorStatusNotificationAsync(request.ConnectorId, StatusNotificationRequestStatus.Unavailable, cancellationToken);
            }
        }
    }

    private async Task HandleCentralSystemResponseAsync(string uniqueId, string jsonPayload, CancellationToken cancellationToken)
    {
        try
        {
            if (!State.PendingRequests.TryRemove(uniqueId, out var ocppMessage))
                throw new InvalidOperationException("No pending request found");

            switch (ocppMessage.Action)
            {
                case Ocpp16ActionTypes.BootNotification:
                    var bootNotificationResponse = JsonConvert.DeserializeObject<BootNotificationResponse>(jsonPayload);
                    await HandleBootNotificationResponseAsync(bootNotificationResponse, cancellationToken);
                    Console.WriteLine($"Boot notification response: {bootNotificationResponse.Status}");
                    break;
                case Ocpp16ActionTypes.Heartbeat:
                    Console.WriteLine("Heartbeat response received.");
                    break;
                case Ocpp16ActionTypes.MeterValues:
                    Console.WriteLine("Meter values response received.");
                    break;
                case Ocpp16ActionTypes.StartTransaction:
                    Console.WriteLine("Start transaction response received.");
                    var startTransactionRequest = JsonConvert.DeserializeObject<StartTransactionRequest>(ocppMessage.JsonPayload);
                    var startTransactionResponse = JsonConvert.DeserializeObject<StartTransactionResponse>(jsonPayload);
                    await HandleStartTransactionResponseAsync(startTransactionRequest, startTransactionResponse, cancellationToken);
                    break;
                case Ocpp16ActionTypes.StopTransaction:
                    Console.WriteLine("Stop transaction response received.");
                    var stopTransactionRequest = JsonConvert.DeserializeObject<StopTransactionRequest>(ocppMessage.JsonPayload);
                    var stopTransactionResponse = JsonConvert.DeserializeObject<StopTransactionResponse>(jsonPayload);
                    await HandleStopTransactionResponseAsync(stopTransactionRequest, stopTransactionResponse, cancellationToken);
                    break;
                case Ocpp16ActionTypes.Authorize:
                    Console.WriteLine("Authorize response received.");
                    var authorizeResponse = JsonConvert.DeserializeObject<AuthorizeResponse>(jsonPayload);
                    var authorizeRequest = JsonConvert.DeserializeObject<AuthorizeRequest>(ocppMessage.JsonPayload);
                    await HandleAuthorizeResponseAsync(uniqueId, authorizeRequest, authorizeResponse, cancellationToken);
                    break;
                case Ocpp16ActionTypes.StatusNotification:
                    Console.WriteLine("Status notification response received.");
                    var statusNotificationResponse = JsonConvert.DeserializeObject<StatusNotificationResponse>(jsonPayload);
                    var statusNotificationRequest = JsonConvert.DeserializeObject<StatusNotificationRequest>(ocppMessage.JsonPayload);
                    await HandleStatusNotificationResponseAsync(statusNotificationRequest, statusNotificationResponse, cancellationToken);
                    break;
                default:
                    throw new InvalidOperationException("Unknown action");
            }
            
            await SaveStateAsync(cancellationToken);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error during handling central system response: {e.Message}");
            throw;
        }
    }

    private async Task HandleStatusNotificationResponseAsync(StatusNotificationRequest statusNotificationRequest, StatusNotificationResponse statusNotificationResponse, CancellationToken cancellationToken)
    {
        State.Connectors.TryAdd(statusNotificationRequest.ConnectorId, new ConnectorState
        {
            ConnectorId = statusNotificationRequest.ConnectorId,
            Status = statusNotificationRequest.Status
        });
        await SaveStateAsync(cancellationToken);
    }

    private async Task HandleAuthorizeResponseAsync(string messageId, AuthorizeRequest authorizeRequest, AuthorizeResponse authorizeResponse, CancellationToken cancellationToken = default)
    {
        if (State.PendingAuthorizeRequests.TryRemove(messageId, out var authCompletionSource))
        {
            authCompletionSource.TrySetResult(authorizeResponse);
        }
        
        if (authorizeResponse.IdTagInfo.Status == IdTagInfoStatus.Accepted)
        {
            State.AuthorizedOcppTags.TryAdd(authorizeRequest.IdTag, authorizeResponse.IdTagInfo);
            await SaveStateAsync(cancellationToken);
        }
    }

    private Task HandleStopTransactionResponseAsync(StopTransactionRequest stopTransactionRequest, StopTransactionResponse stopTransactionResponse, CancellationToken cancellationToken)
    {
        if(stopTransactionResponse.IdTagInfo?.Status == IdTagInfoStatus.Accepted)
        {
            foreach (var (_, connectorState) in State.Connectors)
            {
                if (connectorState.LastTransaction?.TransactionId == stopTransactionRequest.TransactionId)
                {
                    connectorState.Status = StatusNotificationRequestStatus.Available;
                    break;
                }
            }
        }
        else
            Console.WriteLine("Central system rejected stop transaction");
        
        return Task.CompletedTask;
    }

    private async Task HandleStartTransactionResponseAsync(StartTransactionRequest startTransactionRequest, StartTransactionResponse startTransactionResponse, CancellationToken cancellationToken = default)
    {
        if (startTransactionResponse.IdTagInfo.Status == IdTagInfoStatus.Accepted)
        {
            State.Connectors[startTransactionRequest.ConnectorId].LastTransaction = new TransactionState
            {
                TransactionId = startTransactionResponse.TransactionId,
                StartTimestamp = DateTime.UtcNow
            };

            if(startTransactionRequest.ReservationId.HasValue)
                State.Reservations.TryRemove(startTransactionRequest.ReservationId.Value, out _);
            
            _ = Task.Run(async () =>
            {
                try
                {
                    Console.WriteLine("Starting to send meter values...");
                    await SendMeterValuesRequestAsync(startTransactionRequest.ConnectorId,
                        startTransactionResponse.TransactionId, cancellationToken);
                    Console.WriteLine("Meter values sent.");
                    await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
                    Console.WriteLine("Stopping transaction...");
                    await SendStopTransactionRequestAsync(startTransactionResponse.TransactionId, startTransactionRequest.IdTag, cancellationToken);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error in sending meter values: {e.Message}");
                }
            }, cancellationToken);
        }
        else
            Console.WriteLine("Central system rejected start transaction");
    }

    private async Task HandleBootNotificationResponseAsync(BootNotificationResponse bootNotificationResponse, CancellationToken cancellationToken)
    {
        State.BootNotificationStatus = bootNotificationResponse.Status;
        
        if (bootNotificationResponse.Status == BootNotificationResponseStatus.Accepted)
        {
            Console.WriteLine("Central system accepted boot notification");

            for (var connectorId = 1; connectorId < 5; connectorId++)
            {
                await SendConnectorStatusNotificationAsync(connectorId, StatusNotificationRequestStatus.Available, cancellationToken);
            }
            
            _ = Task.Run(async () =>
            {
                try
                {
                    Console.WriteLine("Starting to send heartbeats...");
                    await StartSendingHeartbeatsAsync(30, cancellationToken);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error in sending heartbeats: {e.Message}");
                }
            }, cancellationToken);
        }
    }

    private async Task SendConnectorStatusNotificationAsync(int connectorId, StatusNotificationRequestStatus status, CancellationToken cancellationToken = default)
    {
        try
        {
            var connectorStatusNotification = new StatusNotificationRequest(connectorId, StatusNotificationRequestErrorCode.NoError, status);
            var jsonPayload = JsonConvert.SerializeObject(connectorStatusNotification);
            var messageId = Guid.NewGuid().ToString();
            var ocppMessage = new OcppMessage(OcppMessageTypes.Call, messageId, Ocpp16ActionTypes.StatusNotification, jsonPayload);
            var textMessage = $"[{ocppMessage.MessageType},\"{ocppMessage.UniqueId}\",\"{ocppMessage.Action}\",{ocppMessage.JsonPayload}]";
            await SendMessageAsync(textMessage, cancellationToken);
            State.PendingRequests.TryAdd(messageId, ocppMessage);
            await SaveStateAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while sending heartbeat: {ex.Message}");
        }
    }

    private async Task StartSendingHeartbeatsAsync(int interval, CancellationToken cancellationToken)
    {
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var heartbeatRequest = new HeartbeatRequest();
                var jsonPayload = JsonConvert.SerializeObject(heartbeatRequest);
                var messageId = Guid.NewGuid().ToString();
                var ocppMessage = new OcppMessage(OcppMessageTypes.Call, messageId, Ocpp16ActionTypes.Heartbeat, jsonPayload);
                var textMessage = string.Format("[{0},\"{1}\",\"{2}\",{3}]", ocppMessage.MessageType, ocppMessage.UniqueId, ocppMessage.Action, ocppMessage.JsonPayload);
                await SendMessageAsync(textMessage, cancellationToken);
                State.PendingRequests.TryAdd(messageId, ocppMessage);
                Console.WriteLine("Heartbeat sent.");
                
                await Task.Delay(TimeSpan.FromSeconds(interval), cancellationToken);
            }
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Heartbeat sending has been cancelled.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while sending heartbeat: {ex.Message}");
        }
    }
    
    private async Task SendUpdateToHubAsync(CancellationToken cancellationToken = default)
    {
        if(_hubConnection is null)
            return;
        
        try
        {
            await _hubConnection.SendAsync("HandleUpdateFromChargingStation", cancellationToken);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error during sending update to hub: {e.Message}");
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_webSocket.State != WebSocketState.Closed)
            _webSocket.Abort();
        
        _webSocket.Dispose();
        _connectSemaphore.Dispose();

        if (_hubConnection is not null)
        {
            await _hubConnection.StopAsync();
            await _hubConnection.DisposeAsync();
        }
    }

    public async Task ClearStateAsync()
    {
        var state = new ChargingStationState
        {
            Id = Id.ToString(),
            Connectors = new ConcurrentDictionary<int, ConnectorState>(),
            LastMessageReceived = State.LastMessageReceived
        };
        State = state;
        await SaveStateAsync();
        await SendUpdateToHubAsync();
    }
}