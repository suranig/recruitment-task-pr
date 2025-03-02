namespace Articles.Domain.Commons;

public abstract class BaseEntity<TId> where TId : EntityId
{
    public TId Id { get; protected set; } = null!;

    protected BaseEntity() { }

    protected BaseEntity(TId id)
    {
        Id = id;
    }

    public override bool Equals(object? obj)
    {
        if (obj == null || obj.GetType() != GetType())
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        var other = (BaseEntity<TId>)obj;
        
        if (Id is null || other.Id is null)
        {
            return false;
        }

        return Id.Equals(other.Id);
    }

    public override int GetHashCode()
    {
        return Id?.GetHashCode() ?? 0;
    }

    public static bool operator ==(BaseEntity<TId>? left, BaseEntity<TId>? right)
    {
        if (left is null && right is null)
            return true;

        if (left is null || right is null)
            return false;

        return left.Equals(right);
    }

    public static bool operator !=(BaseEntity<TId>? left, BaseEntity<TId>? right)
    {
        return !(left == right);
    }
} 