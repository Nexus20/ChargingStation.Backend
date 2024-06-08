namespace ChargingStation.InternalCommunication.Services.UserManagement;

public interface IUserHttpService
{
    Task<List<Guid>> GetUserDepotAccessesAsync(Guid userId, CancellationToken cancellationToken = default);
}