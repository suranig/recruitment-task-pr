using Articles.Application.Articles.Commands.UpdateArticle;
using Articles.Application.Commons.Exceptions;
using Articles.Domain.Aggregates.ArticleAggregate;
using Articles.Domain.Interfaces;
using Articles.Domain.ValueObjects;
using FluentAssertions;
using Moq;

namespace Articles.UnitTests.Application.Articles.Commands.UpdateArticle;

public class UpdateArticleCommandHandlerTests
{
    private readonly Mock<IArticleRepository> _mockRepository;
    private readonly UpdateArticleCommandHandler _handler;

    public UpdateArticleCommandHandlerTests()
    {
        _mockRepository = new Mock<IArticleRepository>();
        _handler = new UpdateArticleCommandHandler(_mockRepository.Object);
    }

    [Fact]
    public async Task Handle_ExistingArticle_ShouldUpdateAndSaveArticle()
    {
        var articleId = Guid.NewGuid();
        var command = new UpdateArticleCommand
        {
            Id = articleId,
            Title = "Zaktualizowany tytuł",
            Content = "Zaktualizowana treść"
        };

        var existingArticle = Article.Create(
            ArticleId.Create(articleId),
            "Stary tytuł",
            "Stara treść",
            AuthorId.CreateUnique()
        );

        _mockRepository.Setup(r => r.GetByIdAsync(It.Is<ArticleId>(id => id.Value == articleId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingArticle);

        await _handler.Handle(command, CancellationToken.None);

        existingArticle.Title.Should().Be(command.Title);
        existingArticle.Content.Should().Be(command.Content);
        _mockRepository.Verify(r => r.UpdateAsync(existingArticle, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NonExistingArticle_ShouldThrowNotFoundException()
    {
        var articleId = Guid.NewGuid();
        var command = new UpdateArticleCommand
        {
            Id = articleId,
            Title = "Zaktualizowany tytuł",
            Content = "Zaktualizowana treść"
        };

        _mockRepository.Setup(r => r.GetByIdAsync(It.Is<ArticleId>(id => id.Value == articleId), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Article?)null);

        await _handler.Invoking(h => h.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<NotFoundException>()
            .WithMessage($"*{articleId}*");
    }
} 