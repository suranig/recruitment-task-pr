using Articles.Application.Articles.Commands.AddTagToArticle;
using Articles.Application.Commons.Exceptions;
using Articles.Domain.Aggregates.ArticleAggregate;
using Articles.Domain.Interfaces;
using Articles.Domain.ValueObjects;
using FluentAssertions;
using Moq;

namespace Articles.UnitTests.Application.Articles.Commands.AddTagToArticle;

public class AddTagToArticleCommandHandlerTests
{
    private readonly Mock<IArticleRepository> _mockRepository;
    private readonly AddTagToArticleCommandHandler _handler;

    public AddTagToArticleCommandHandlerTests()
    {
        _mockRepository = new Mock<IArticleRepository>();
        _handler = new AddTagToArticleCommandHandler(_mockRepository.Object);
    }

    [Fact]
    public async Task Handle_ExistingArticle_ShouldAddTagToArticle()
    {
        var articleId = Guid.NewGuid();
        var tagName = "test-tag";
        var command = new AddTagToArticleCommand 
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

        _mockRepository.Setup(r => r.GetByIdAsync(It.Is<ArticleId>(id => id.Value == articleId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingArticle);

        await _handler.Handle(command, CancellationToken.None);

        existingArticle.Tags.Should().ContainSingle(t => t.Name.Value == tagName);
        _mockRepository.Verify(r => r.UpdateAsync(existingArticle, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NonExistingArticle_ShouldThrowNotFoundException()
    {
        var articleId = Guid.NewGuid();
        var command = new AddTagToArticleCommand 
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