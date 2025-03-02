using Articles.Domain.Commons;

namespace Articles.Domain.ValueObjects;

public record AuthorId : EntityId
{
    private AuthorId(Guid value) : base(value)
    {
    }

    public static AuthorId Create(Guid value)
    {
        return new AuthorId(value);
    }

    public static AuthorId CreateUnique()
    {
        return new AuthorId(Guid.NewGuid());
    }
} 