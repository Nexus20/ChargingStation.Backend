using AutoMapper;
using ChargingStation.Common.Exceptions;
using ChargingStation.Common.Messages_OCPP16;
using ChargingStation.Connectors.Models.Requests;
using ChargingStation.Connectors.Models.Responses;
using ChargingStation.Connectors.Specifications;
using ChargingStation.Domain.Entities;
using ChargingStation.Infrastructure.Repositories;
using ConnectorStatus = ChargingStation.Domain.Entities.ConnectorStatus;

namespace ChargingStation.Connectors.Services;

public class ConnectorService : IConnectorService
{
    private readonly IRepository<Connector> _connectorRepository;
    private readonly IRepository<ConnectorStatus> _connectorStatusRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<ConnectorService> _logger;

    public ConnectorService(IRepository<Connector> connectorRepository, IMapper mapper, ILogger<ConnectorService> logger, IRepository<ConnectorStatus> connectorStatusRepository)
    {
        _connectorRepository = connectorRepository;
        _connectorStatusRepository = connectorStatusRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task UpdateConnectorStatusAsync(UpdateConnectorStatusRequest request, CancellationToken cancellationToken = default)
    {
        var specification = new GetConnectorsSpecification(request);
        
        var connector = await _connectorRepository.GetFirstOrDefaultAsync(specification, cancellationToken);
        
        if (connector is null)
        {
            throw new NotFoundException($"Connector with ChargePointId {request.ChargePointId} and ConnectorId {request.ConnectorId} not found");
        }
        
        var statusToCreate = _mapper.Map<ConnectorStatus>(request);
        await _connectorStatusRepository.AddAsync(statusToCreate, cancellationToken);
        await _connectorStatusRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task<ConnectorResponse> GetOrCreateConnectorAsync(GetOrCreateConnectorRequest request, CancellationToken cancellationToken = default)
    {
        var specification = new GetConnectorsSpecification(request);
        
        var connector = await _connectorRepository.GetFirstOrDefaultAsync(specification, cancellationToken);
        
        if (connector is not null)
        {
            var existingConnectorResponse = _mapper.Map<ConnectorResponse>(connector);
            return existingConnectorResponse;
        }
        
        _logger.LogInformation("Connector not found. Creating new connector");
        
        var connectorToCreate = _mapper.Map<Connector>(request);
        
        await _connectorRepository.AddAsync(connectorToCreate, cancellationToken);
        await _connectorRepository.SaveChangesAsync(cancellationToken);
        
        var createdConnectorResponse = _mapper.Map<ConnectorResponse>(connectorToCreate);
        return createdConnectorResponse;
    }

    public async Task<ConnectorResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var connector = await _connectorRepository.GetByIdAsync(id, cancellationToken);
        
        if (connector is null)
            throw new NotFoundException(nameof(Connector), id);
        
        var connectorResponse = _mapper.Map<ConnectorResponse>(connector);
        return connectorResponse;
    }

    public async Task<StatusNotificationResponse> ProcessStatusNotificationAsync(StatusNotificationRequest request, Guid chargePointId, CancellationToken cancellationToken = default)
    {
        var response = new StatusNotificationResponse();

        var updateStatusRequest = new UpdateConnectorStatusRequest
        {
            ChargePointId = chargePointId,
            ConnectorId = request.ConnectorId,
            Status = request.Status.ToString(),
            StatusTimestamp = request.Timestamp?.UtcDateTime ?? DateTime.UtcNow,
            ErrorCode = request.ErrorCode.ToString(),
            Info = request.Info,
            VendorId = request.VendorId,
            VendorErrorCode = request.VendorErrorCode
        };
        
        await UpdateConnectorStatusAsync(updateStatusRequest, cancellationToken);
        
        return response;
    }
}