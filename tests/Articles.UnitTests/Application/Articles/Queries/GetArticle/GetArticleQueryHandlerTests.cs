using Articles.Application.Articles.Queries.GetArticle;
using Articles.Domain.Aggregates.ArticleAggregate;
using Articles.Domain.Interfaces;
using Articles.Domain.ValueObjects;
using FluentAssertions;
using Moq;

namespace Articles.UnitTests.Application.Articles.Queries.GetArticle;

public class GetArticleQueryHandlerTests
{
    private readonly Mock<IArticleRepository> _mockRepository;
    private readonly GetArticleQueryHandler _handler;

    public GetArticleQueryHandlerTests()
    {
        _mockRepository = new Mock<IArticleRepository>();
        _handler = new GetArticleQueryHandler(_mockRepository.Object);
    }

    [Fact]
    public async Task Handle_ExistingArticle_ShouldReturnArticleDetails()
    {
        var articleId = Guid.NewGuid();
        var authorId = AuthorId.CreateUnique();
        
        var article = Article.Create(
            ArticleId.Create(articleId),
            "Testowy artykuł",
            "Treść testowego artykułu",
            authorId
        );
        
        article.AddTag(TagName.Create("test"));
        
        _mockRepository.Setup(r => r.GetByIdAsync(It.Is<ArticleId>(id => id.Value == articleId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(article);

        var query = new GetArticleQuery { Id = articleId };

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(articleId);
        result.Title.Should().Be("Testowy artykuł");
        result.Content.Should().Be("Treść testowego artykułu");
        result.Tags.Should().ContainSingle(t => t == "test");
    }

    [Fact]
    public async Task Handle_NonExistingArticle_ShouldReturnNull()
    {
        var articleId = Guid.NewGuid();
        
        _mockRepository.Setup(r => r.GetByIdAsync(It.Is<ArticleId>(id => id.Value == articleId), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Article?)null);

        var query = new GetArticleQuery { Id = articleId };

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().BeNull();
    }
} 