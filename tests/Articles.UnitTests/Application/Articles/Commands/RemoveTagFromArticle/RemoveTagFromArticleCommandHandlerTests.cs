using Articles.Application.Articles.Commands.RemoveTagFromArticle;
using Articles.Application.Commons.Exceptions;
using Articles.Domain.Aggregates.ArticleAggregate;
using Articles.Domain.Interfaces;
using Articles.Domain.ValueObjects;
using FluentAssertions;
using Moq;

namespace Articles.UnitTests.Application.Articles.Commands.RemoveTagFromArticle;

public class RemoveTagFromArticleCommandHandlerTests
{
    private readonly Mock<IArticleRepository> _mockRepository;
    private readonly RemoveTagFromArticleCommandHandler _handler;

    public RemoveTagFromArticleCommandHandlerTests()
    {
        _mockRepository = new Mock<IArticleRepository>();
        _handler = new RemoveTagFromArticleCommandHandler(_mockRepository.Object);
    }

    [Fact]
    public async Task Handle_ExistingArticleWithTag_ShouldRemoveTagFromArticle()
    {
        var articleId = Guid.NewGuid();
        var tagName = "test-tag";
        var command = new RemoveTagFromArticleCommand 
        { 
            ArticleId = articleId, 
            TagName = tagName 
        };

        var existingArticle = Article.Create(
            ArticleId.Create(articleId),
            "Tytuł artykułu",
            "Treść artykułu",
            AuthorId.CreateUnique()
        );
        existingArticle.AddTag(TagName.Create(tagName));

        _mockRepository.Setup(r => r.GetByIdAsync(It.Is<ArticleId>(id => id.Value == articleId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingArticle);

        await _handler.Handle(command, CancellationToken.None);

        existingArticle.Tags.Should().BeEmpty();
        _mockRepository.Verify(r => r.UpdateAsync(existingArticle, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NonExistingArticle_ShouldThrowNotFoundException()
    {
        var articleId = Guid.NewGuid();
        var command = new RemoveTagFromArticleCommand 
        { 
            ArticleId = articleId, 
            TagName = "test-tag" 
        };

        _mockRepository.Setup(r => r.GetByIdAsync(It.Is<ArticleId>(id => id.Value == articleId), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Article?)null);

        await _handler.Invoking(h => h.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<NotFoundException>()
            .WithMessage($"*{articleId}*");
    }
} 