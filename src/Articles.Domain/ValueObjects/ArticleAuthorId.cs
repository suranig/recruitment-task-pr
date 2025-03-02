using Articles.Domain.Commons;

namespace Articles.Domain.ValueObjects;

public record ArticleAuthorId : EntityId
{
    private ArticleAuthorId(Guid value) : base(value)
    {
    }

    public static ArticleAuthorId Create(Guid value)
    {
        return new ArticleAuthorId(value);
    }

    public static ArticleAuthorId CreateUnique()
    {
        return new ArticleAuthorId(Guid.NewGuid());
    }
} 