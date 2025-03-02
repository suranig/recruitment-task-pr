using Articles.Domain.ValueObjects;

namespace Articles.Domain.Events;

public class ArticlePublishedEvent : DomainEvent
{
    public ArticleId ArticleId { get; }
    
    public ArticlePublishedEvent(ArticleId articleId)
    {
        ArticleId = articleId;
    }
} 