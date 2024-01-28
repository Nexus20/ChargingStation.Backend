namespace ChargingStation.Domain.Abstract;

/// <summary>
/// Base class for all entities.
/// </summary>
public abstract class Entity : IEquatable<Entity>
{
    /// <summary>
    /// Unique identifier of the entity.
    /// </summary>
    public Guid Id { get; set; }

    public override bool Equals(object? obj)
    {
        if (obj is Entity entity)
            return ((IEquatable<Entity>)this).Equals(entity);

        return false;
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
    
    public bool Equals(Entity? other)
    {
        if (other is null)
            return false;

        if (other.GetType() != GetType())
            return false;
        
        return other.Id == Id;
    }
    
    public static bool operator ==(Entity? left, Entity? right)
    {
        return (left is not null && right is not null && left.Equals(right)) || (left is null && right is null);
    }

    public static bool operator !=(Entity left, Entity right)
    {
        return !(left == right);
    }
}