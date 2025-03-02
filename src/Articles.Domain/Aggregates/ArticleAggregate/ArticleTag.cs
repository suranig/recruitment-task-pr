using Articles.Domain.Commons;
using Articles.Domain.ValueObjects;

namespace Articles.Domain.Aggregates.ArticleAggregate;

public class ArticleTag : BaseEntity<ArticleTagId>
{
    public Article Article { get; private set; } = null!;
    public TagName Name { get; private set; } = null!;
    
    private ArticleTag() { }
    
    private ArticleTag(ArticleTagId id, Article article, TagName name) : base(id)
    {
        Article = article;
        Name = name;
    }
    
    public static ArticleTag Create(Article article, TagName name)
    {
        return new ArticleTag(ArticleTagId.CreateUnique(), article, name);
    }
} 