using Articles.Application.Articles.Commands.DeleteArticle;
using Articles.Application.Commons.Exceptions;
using Articles.Domain.Aggregates.ArticleAggregate;
using Articles.Domain.Interfaces;
using Articles.Domain.ValueObjects;
using FluentAssertions;
using Moq;

namespace Articles.UnitTests.Application.Articles.Commands.DeleteArticle;

public class DeleteArticleCommandHandlerTests
{
    private readonly Mock<IArticleRepository> _mockRepository;
    private readonly DeleteArticleCommandHandler _handler;

    public DeleteArticleCommandHandlerTests()
    {
        _mockRepository = new Mock<IArticleRepository>();
        _handler = new DeleteArticleCommandHandler(_mockRepository.Object);
    }

    [Fact]
    public async Task Handle_ExistingArticle_ShouldDeleteArticle()
    {
        var articleId = Guid.NewGuid();
        var command = new DeleteArticleCommand { Id = articleId };

        var existingArticle = Article.Create(
            ArticleId.Create(articleId),
            "Tytuł artykułu",
            "Treść artykułu",
            AuthorId.CreateUnique()
        );

        _mockRepository.Setup(r => r.GetByIdAsync(It.Is<ArticleId>(id => id.Value == articleId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingArticle);

        await _handler.Handle(command, CancellationToken.None);

        _mockRepository.Verify(r => r.DeleteAsync(It.Is<ArticleId>(id => id.Value == articleId), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NonExistingArticle_ShouldThrowNotFoundException()
    {
        var articleId = Guid.NewGuid();
        var command = new DeleteArticleCommand { Id = articleId };

        _mockRepository.Setup(r => r.GetByIdAsync(It.Is<ArticleId>(id => id.Value == articleId), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Article?)null);

        await _handler.Invoking(h => h.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<NotFoundException>()
            .WithMessage($"*{articleId}*");
    }
} 