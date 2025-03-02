using Articles.Domain.Commons;
using Articles.Domain.ValueObjects;

namespace Articles.Domain.Aggregates.ArticleAggregate;

public class ArticleAuthor : BaseEntity<ArticleAuthorId>
{
    public Article Article { get; private set; } = null!;
    public AuthorId AuthorId { get; private set; } = null!;
    
    private ArticleAuthor() { }
    
    private ArticleAuthor(ArticleAuthorId id, Article article, AuthorId authorId) : base(id)
    {
        Article = article;
        AuthorId = authorId;
    }
    
    public static ArticleAuthor Create(Article article, AuthorId authorId)
    {
        return new ArticleAuthor(ArticleAuthorId.CreateUnique(), article, authorId);
    }
} 