using Articles.Application.Articles.Commands.PublishArticle;
using Articles.Application.Commons.Exceptions;
using Articles.Domain.Aggregates.ArticleAggregate;
using Articles.Domain.Interfaces;
using Articles.Domain.ValueObjects;
using FluentAssertions;
using Moq;

namespace Articles.UnitTests.Application.Articles.Commands.PublishArticle;

public class PublishArticleCommandHandlerTests
{
    private readonly Mock<IArticleRepository> _mockRepository;
    private readonly PublishArticleCommandHandler _handler;

    public PublishArticleCommandHandlerTests()
    {
        _mockRepository = new Mock<IArticleRepository>();
        _handler = new PublishArticleCommandHandler(_mockRepository.Object);
    }

    [Fact]
    public async Task Handle_ExistingArticle_ShouldPublishArticle()
    {
        var articleId = Guid.NewGuid();
        var command = new PublishArticleCommand { Id = articleId };

        var existingArticle = Article.Create(
            ArticleId.Create(articleId),
            "Tytuł artykułu",
            "Treść artykułu",
            AuthorId.CreateUnique()
        );

        _mockRepository.Setup(r => r.GetByIdAsync(It.Is<ArticleId>(id => id.Value == articleId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingArticle);

        await _handler.Handle(command, CancellationToken.None);

        existingArticle.Status.Should().Be(PublicationStatus.Published);
        existingArticle.PublishedAt.Should().NotBeNull();
        _mockRepository.Verify(r => r.UpdateAsync(existingArticle, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NonExistingArticle_ShouldThrowNotFoundException()
    {
        var articleId = Guid.NewGuid();
        var command = new PublishArticleCommand { Id = articleId };

        _mockRepository.Setup(r => r.GetByIdAsync(It.Is<ArticleId>(id => id.Value == articleId), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Article?)null);

        await _handler.Invoking(h => h.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<NotFoundException>()
            .WithMessage($"*{articleId}*");
    }
} 