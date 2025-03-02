using Articles.Domain.Aggregates.ArticleAggregate;
using Articles.Domain.Events;
using Articles.Domain.Exceptions;
using Articles.Domain.ValueObjects;
using FluentAssertions;

namespace Articles.UnitTests.Domain.Aggregates.ArticleAggregate;

public class ArticleTests
{
    [Fact]
    public void Create_WithValidParameters_CreatesArticle()
    {
        var articleId = ArticleId.Create(Guid.NewGuid());
        var title = "Test Article";
        var content = "Test Content";
        var authorId = AuthorId.Create(Guid.NewGuid());
        
        var article = Article.Create(articleId, title, content, authorId);
        
        article.Should().NotBeNull();
        article.Id.Should().Be(articleId);
        article.Title.Should().Be(title);
        article.Content.Should().Be(content);
        article.Status.Should().Be(PublicationStatus.Draft);
        article.Authors.Should().HaveCount(1);
        article.Authors.First().AuthorId.Should().Be(authorId);
        article.DomainEvents.Should().ContainSingle(e => e is ArticleCreatedEvent);
    }
    
    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void Create_WithInvalidTitle_ThrowsDomainException(string invalidTitle)
    {
        var articleId = ArticleId.Create(Guid.NewGuid());
        var content = "Test Content";
        var authorId = AuthorId.Create(Guid.NewGuid());
        
        Action act = () => Article.Create(articleId, invalidTitle, content, authorId);
        act.Should().Throw<DomainException>().WithMessage("*title*");
    }
    
    [Fact]
    public void AddTag_WithValidTag_AddsTagToArticle()
    {
        var article = CreateValidArticle();
        var tagName = TagName.Create("test-tag");
        
        article.AddTag(tagName);
        
        article.Tags.Should().HaveCount(1);
        article.Tags.First().Name.Should().Be(tagName);
    }
    
    [Fact]
    public void RemoveTag_WithExistingTag_RemovesTagFromArticle()
    {
        var article = CreateValidArticle();
        var tagName = TagName.Create("test-tag");
        article.AddTag(tagName);
        
        article.RemoveTag(tagName);
        
        article.Tags.Should().BeEmpty();
    }
    
    [Fact]
    public void Publish_WhenInDraftStatus_ChangesStatusToPublished()
    {
        var article = CreateValidArticle();
        
        article.Publish();
        
        article.Status.Should().Be(PublicationStatus.Published);
        article.PublishedAt.Should().NotBeNull();
        article.DomainEvents.Should().ContainSingle(e => e is ArticlePublishedEvent);
    }
    
    [Fact]
    public void Publish_WhenAlreadyPublished_ThrowsDomainException()
    {
        var article = CreateValidArticle();
        article.Publish();
        
        Action act = () => article.Publish();
        act.Should().Throw<DomainException>().WithMessage("*already published*");
    }
    
    [Fact]
    public void Unpublish_WhenPublished_ChangesStatusToDraft()
    {
        var article = CreateValidArticle();
        article.Publish();
        
        article.Unpublish();
        
        article.Status.Should().Be(PublicationStatus.Draft);
        article.PublishedAt.Should().BeNull();
    }
    
    [Fact]
    public void Unpublish_WhenInDraftStatus_ThrowsDomainException()
    {
        var article = CreateValidArticle();
        
        Action act = () => article.Unpublish();
        act.Should().Throw<DomainException>().WithMessage("*not published*");
    }
    
    [Fact]
    public void UpdateContent_WithValidContent_UpdatesArticleContent()
    {
        var article = CreateValidArticle();
        var newContent = "Updated content";
        
        article.UpdateContent(newContent);
        
        article.Content.Should().Be(newContent);
    }
    
    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void UpdateContent_WithInvalidContent_ThrowsDomainException(string invalidContent)
    {
        var article = CreateValidArticle();
        
        Action act = () => article.UpdateContent(invalidContent);
        act.Should().Throw<DomainException>().WithMessage("*content*");
    }
    
    private Article CreateValidArticle()
    {
        var articleId = ArticleId.Create(Guid.NewGuid());
        var title = "Test Article";
        var content = "Test Content";
        var authorId = AuthorId.Create(Guid.NewGuid());
        
        return Article.Create(articleId, title, content, authorId);
    }
} 