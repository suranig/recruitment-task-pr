namespace Articles.Domain.Commons;

public abstract record EntityId
{
    public Guid Value { get; }

    protected EntityId(Guid value)
    {
        Value = value;
    }
} 