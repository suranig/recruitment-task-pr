using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Articles.Application.Articles.Commands.AddTagToArticle;
using Articles.Application.Articles.Commands.CreateArticle;
using Articles.Application.Articles.Queries.GetArticle;
using Articles.Domain.Aggregates.ArticleAggregate;
using Articles.Domain.ValueObjects;
using FluentAssertions;
using Moq;

namespace Articles.IntegrationTests.Controllers;

public class ArticlesControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

    public ArticlesControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateArticle_ValidCommand_ReturnsCreatedWithId()
    {
        var command = new CreateArticleCommand
        {
            Title = "Test Article",
            Content = "Test Content",
            AuthorId = Guid.NewGuid()
        };

        var response = await _client.PostAsJsonAsync("/v1/Articles", command);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var content = await response.Content.ReadFromJsonAsync<CreateArticleResponse>(_jsonOptions);
        content.Should().NotBeNull();
        content!.Id.Should().NotBeEmpty();
        content.Title.Should().Be(command.Title);
    }

    [Fact]
    public async Task AddTag_ValidCommand_ReturnsNoContent()
    {
        var articleId = await CreateTestArticle();
        var command = new AddTagToArticleCommand
        {
            ArticleId = articleId,
            TagName = "test-tag"
        };

        var response = await _client.PostAsJsonAsync($"/v1/Articles/{articleId}/tags", command);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var articleResponse = await _client.GetAsync($"/v1/Articles/{articleId}");
        var article = await articleResponse.Content.ReadFromJsonAsync<ArticleDetailsDto>(_jsonOptions);
        article.Should().NotBeNull();
        article!.Tags.Should().Contain("test-tag");
    }

    [Fact]
    public async Task RemoveTag_ExistingTag_ReturnsNoContent()
    {
        var articleId = await CreateTestArticle();
        var tagName = "test-tag";
        
        var addCommand = new AddTagToArticleCommand
        {
            ArticleId = articleId,
            TagName = tagName
        };
        await _client.PostAsJsonAsync($"/v1/Articles/{articleId}/tags", addCommand);

        var response = await _client.DeleteAsync($"/v1/Articles/{articleId}/tags/{tagName}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var articleResponse = await _client.GetAsync($"/v1/Articles/{articleId}");
        var article = await articleResponse.Content.ReadFromJsonAsync<ArticleDetailsDto>(_jsonOptions);
        article.Should().NotBeNull();
        article!.Tags.Should().NotContain(tagName);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenArticleDoesNotExist()
    {
        var nonExistentId = Guid.NewGuid();

        var response = await _client.GetAsync($"/v1/articles/{nonExistentId}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetById_ReturnsOk_WhenArticleExists()
    {
        var articleId = Guid.NewGuid();
        var authorId = Guid.NewGuid();
        
        var article = Article.Create(
            ArticleId.Create(articleId),
            "Test Article",
            "Test Content",
            AuthorId.Create(authorId)
        );
        
        _factory._mockArticleRepository.Setup(r => r.GetByIdAsync(It.Is<ArticleId>(id => id.Value == articleId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(article);

        var response = await _client.GetAsync($"/v1/articles/{articleId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ArticleDetailsDto>();
        result.Should().NotBeNull();
        result!.Id.Should().Be(articleId);
        result.Title.Should().Be("Test Article");
        result.Content.Should().Be("Test Content");
    }

    private async Task<Guid> CreateTestArticle()
    {
        var command = new CreateArticleCommand
        {
            Title = "Test Article for Tags",
            Content = "Test Content",
            AuthorId = Guid.NewGuid()
        };

        var response = await _client.PostAsJsonAsync("/v1/Articles", command);
        var content = await response.Content.ReadFromJsonAsync<CreateArticleResponse>(_jsonOptions);
        return content!.Id;
    }
} 