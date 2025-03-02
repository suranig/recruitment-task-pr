using Articles.Application.Articles.Commands.CreateArticle;
using Articles.Domain.Aggregates.ArticleAggregate;
using Articles.Domain.Interfaces;
using Articles.Domain.ValueObjects;
using FluentAssertions;
using Moq;

namespace Articles.UnitTests.Application.Articles.Commands.CreateArticle;

public class CreateArticleCommandHandlerTests
{
    private readonly Mock<IArticleRepository> _mockRepository;
    private readonly CreateArticleCommandHandler _handler;

    public CreateArticleCommandHandlerTests()
    {
        _mockRepository = new Mock<IArticleRepository>();
        _handler = new CreateArticleCommandHandler(_mockRepository.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldCreateAndSaveArticle()
    {
        var command = new CreateArticleCommand
        {
            Title = "Testowy artykuł",
            Content = "Treść testowego artykułu",
            AuthorId = Guid.NewGuid()
        };

        var createdArticle = Article.Create(
            ArticleId.CreateUnique(),
            command.Title,
            command.Content,
            AuthorId.Create(command.AuthorId)
        );

        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Article>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdArticle);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
        result.Title.Should().Be(command.Title);

        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Article>(), It.IsAny<CancellationToken>()), Times.Once);
    }
} 