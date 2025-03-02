using Articles.Domain.Commons;

namespace Articles.Domain.ValueObjects;

public record ArticleTagId : EntityId
{
    private ArticleTagId(Guid value) : base(value)
    {
    }

    public static ArticleTagId Create(Guid value)
    {
        return new ArticleTagId(value);
    }

    public static ArticleTagId CreateUnique()
    {
        return new ArticleTagId(Guid.NewGuid());
    }
} 