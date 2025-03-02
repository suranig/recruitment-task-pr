using Articles.Domain.Commons;
using Articles.Domain.Events;
using Articles.Domain.Exceptions;
using Articles.Domain.ValueObjects;

namespace Articles.Domain.Aggregates.ArticleAggregate;

public class Article : BaseAggregateRoot<ArticleId>
{
    private readonly List<ArticleAuthor> _authors = new();
    private readonly List<ArticleTag> _tags = new();
    
    public string Title { get; private set; } = string.Empty;
    public string Content { get; private set; } = string.Empty;
    public PublicationStatus Status { get; private set; }
    public DateTime? PublishedAt { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    
    public IReadOnlyCollection<ArticleAuthor> Authors => _authors.AsReadOnly();
    public IReadOnlyCollection<ArticleTag> Tags => _tags.AsReadOnly();
    
    private Article() { }
    
    private Article(ArticleId id, string title, string content, AuthorId authorId) : base(id)
    {
        Title = title;
        Content = content;
        Status = PublicationStatus.Draft;
        CreatedAt = DateTime.UtcNow;
        
        _authors.Add(ArticleAuthor.Create(this, authorId));
        
        AddDomainEvent(new ArticleCreatedEvent(Id));
    }
    
    public static Article Create(ArticleId id, string title, string content, AuthorId authorId)
    {
        ValidateTitle(title);
        ValidateContent(content);
        
        return new Article(id, title, content, authorId);
    }
    
    public void UpdateTitle(string title)
    {
        ValidateTitle(title);
        
        Title = title;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void UpdateContent(string content)
    {
        ValidateContent(content);
        
        Content = content;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void AddAuthor(AuthorId authorId)
    {
        if (_authors.Any(a => a.AuthorId == authorId))
        {
            throw new DomainException($"Author with id {authorId} is already assigned to this article");
        }
        
        _authors.Add(ArticleAuthor.Create(this, authorId));
    }
    
    public void RemoveAuthor(AuthorId authorId)
    {
        if (_authors.Count <= 1)
        {
            throw new DomainException("Cannot remove the last author from the article");
        }
        
        var author = _authors.FirstOrDefault(a => a.AuthorId == authorId);
        if (author == null)
        {
            throw new DomainException($"Author with id {authorId} is not assigned to this article");
        }
        
        _authors.Remove(author);
    }
    
    public void AddTag(TagName tagName)
    {
        if (_tags.Any(t => t.Name == tagName))
        {
            throw new DomainException($"Tag with name {tagName} is already assigned to this article");
        }
        
        _tags.Add(ArticleTag.Create(this, tagName));
    }
    
    public void RemoveTag(TagName tagName)
    {
        var tag = _tags.FirstOrDefault(t => t.Name == tagName);
        if (tag == null)
        {
            throw new DomainException($"Tag with name {tagName} is not assigned to this article");
        }
        
        _tags.Remove(tag);
    }
    
    public void Publish()
    {
        if (Status == PublicationStatus.Published)
        {
            throw new DomainException("Article is already published");
        }
        
        Status = PublicationStatus.Published;
        PublishedAt = DateTime.UtcNow;
        
        AddDomainEvent(new ArticlePublishedEvent(Id));
    }
    
    public void Unpublish()
    {
        if (Status != PublicationStatus.Published)
        {
            throw new DomainException("Article is not published");
        }
        
        Status = PublicationStatus.Draft;
        PublishedAt = null;
    }
    
    private static void ValidateTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new DomainException("Article title cannot be empty");
        }
    }
    
    private static void ValidateContent(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            throw new DomainException("Article content cannot be empty");
        }
    }
} 