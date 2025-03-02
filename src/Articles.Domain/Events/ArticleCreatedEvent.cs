using Articles.Domain.ValueObjects;

namespace Articles.Domain.Events;

public class ArticleCreatedEvent : DomainEvent
{
    public ArticleId ArticleId { get; }
    
    public ArticleCreatedEvent(ArticleId articleId)
    {
        ArticleId = articleId;
    }
} 