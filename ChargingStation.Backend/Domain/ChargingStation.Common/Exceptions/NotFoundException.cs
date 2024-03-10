namespace ChargingStation.Common.Exceptions;

[Serializable]
public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message)
    { }

    public NotFoundException(string entityTypeName, object key) : base($"Entity {entityTypeName} with key {key} not found")
    { }
}