using Articles.Domain.Commons;

namespace Articles.Domain.ValueObjects;

public record ArticleId : EntityId
{
    private ArticleId(Guid value) : base(value)
    {
    }

    public static ArticleId Create(Guid value)
    {
        return new ArticleId(value);
    }

    public static ArticleId CreateUnique()
    {
        return new ArticleId(Guid.NewGuid());
    }
} 