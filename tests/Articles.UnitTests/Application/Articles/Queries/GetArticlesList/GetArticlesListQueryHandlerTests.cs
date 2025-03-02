using Articles.Application.Articles.Queries.GetArticlesList;
using Articles.Domain.Aggregates.ArticleAggregate;
using Articles.Domain.Interfaces;
using Articles.Domain.ValueObjects;
using FluentAssertions;
using Moq;

namespace Articles.UnitTests.Application.Articles.Queries.GetArticlesList;

public class GetArticlesListQueryHandlerTests
{
    private readonly Mock<IArticleRepository> _mockRepository;
    private readonly GetArticlesListQueryHandler _handler;

    public GetArticlesListQueryHandlerTests()
    {
        _mockRepository = new Mock<IArticleRepository>();
        _handler = new GetArticlesListQueryHandler(_mockRepository.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnPaginatedArticles()
    {
        var query = new GetArticlesListQuery
        {
            PageNumber = 1,
            PageSize = 10
        };

        var articles = new List<Article>
        {
            CreateTestArticle("Artykuł 1"),
            CreateTestArticle("Artykuł 2")
        };

        _mockRepository.Setup(r => r.GetAllAsync(query.PageNumber, query.PageSize, It.IsAny<CancellationToken>()))
            .ReturnsAsync((articles, 2));

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().NotBeNull();
        result.Items.Should().HaveCount(2);
        result.TotalCount.Should().Be(2);
        result.PageNumber.Should().Be(1);
        result.PageSize.Should().Be(10);
        result.TotalPages.Should().Be(1);
        result.HasNextPage.Should().BeFalse();
        result.HasPreviousPage.Should().BeFalse();
    }

    private Article CreateTestArticle(string title)
    {
        var articleId = ArticleId.CreateUnique();
        var authorId = AuthorId.CreateUnique();
        return Article.Create(articleId, title, "Treść testowa", authorId);
    }
} 