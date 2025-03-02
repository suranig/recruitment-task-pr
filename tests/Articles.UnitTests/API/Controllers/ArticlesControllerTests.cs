using Articles.API.Controllers;
using Articles.Application.Articles.Commands.CreateArticle;
using Articles.Application.Articles.Commands.DeleteArticle;
using Articles.Application.Articles.Commands.UpdateArticle;
using Articles.Application.Articles.Queries.GetArticle;
using Articles.Application.Articles.Queries.GetArticlesList;
using Articles.Application.Commons.Models;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Microsoft.AspNetCore.Http;
using Articles.Application.Articles.Commands.PublishArticle;
using Articles.Application.Articles.Commands.UnpublishArticle;

namespace Articles.UnitTests.API.Controllers;

public class ArticlesControllerTests
{
    private readonly Mock<IMediator> _mockMediator;
    private readonly ArticlesController _controller;

    public ArticlesControllerTests()
    {
        _mockMediator = new Mock<IMediator>();
        _controller = new ArticlesController(_mockMediator.Object);
    }

    [Fact]
    public async Task GetById_ExistingArticle_ReturnsOkWithArticle()
    {
        var articleId = Guid.NewGuid();
        var articleDto = new ArticleDetailsDto
        {
            Id = articleId,
            Title = "Testowy artykuł",
            Content = "Treść testowego artykułu"
        };

        _mockMediator.Setup(m => m.Send(It.Is<GetArticleQuery>(q => q.Id == articleId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(articleDto);

        var result = await _controller.GetById(articleId);

        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedArticle = okResult.Value.Should().BeAssignableTo<ArticleDetailsDto>().Subject;
        returnedArticle.Should().Be(articleDto);
    }

    [Fact]
    public async Task GetById_NonExistingArticle_ReturnsNotFound()
    {
        var articleId = Guid.NewGuid();

        _mockMediator.Setup(m => m.Send(It.Is<GetArticleQuery>(q => q.Id == articleId), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ArticleDetailsDto?)null);

        var result = await _controller.GetById(articleId);

        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task GetList_ReturnsOkWithPaginatedList()
    {
        var query = new GetArticlesListQuery { PageNumber = 1, PageSize = 10 };
        var paginatedList = new PaginatedList<ArticleListItemDto>(
            new List<ArticleListItemDto> { new() { Id = Guid.NewGuid(), Title = "Testowy artykuł" } },
            1, 1, 10);

        _mockMediator.Setup(m => m.Send(It.IsAny<GetArticlesListQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(paginatedList);

        var result = await _controller.GetList(query);

        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedList = okResult.Value.Should().BeAssignableTo<PaginatedList<ArticleListItemDto>>().Subject;
        returnedList.Should().Be(paginatedList);
    }

    [Fact]
    public async Task Create_ValidCommand_ReturnsCreatedWithId()
    {
        var command = new CreateArticleCommand
        {
            Title = "Nowy artykuł",
            Content = "Treść nowego artykułu",
            AuthorId = Guid.NewGuid()
        };

        var response = new CreateArticleResponse
        {
            Id = Guid.NewGuid(),
            Title = command.Title,
            CreatedAt = DateTime.UtcNow
        };

        _mockMediator.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var result = await _controller.Create(command);

        var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdResult.ActionName.Should().Be(nameof(ArticlesController.GetById));
        createdResult.RouteValues.Should().ContainKey("id").WhoseValue.Should().Be(response.Id);
        var returnedResponse = createdResult.Value.Should().BeAssignableTo<CreateArticleResponse>().Subject;
        returnedResponse.Should().Be(response);
    }

    [Fact]
    public async Task Update_ValidCommand_ReturnsNoContent()
    {
        var articleId = Guid.NewGuid();
        var command = new UpdateArticleCommand
        {
            Id = articleId,
            Title = "Zaktualizowany tytuł",
            Content = "Zaktualizowana treść"
        };

        var result = await _controller.Update(articleId, command);

        result.Should().BeOfType<NoContentResult>();
        _mockMediator.Verify(m => m.Send(command, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Delete_ExistingArticle_ReturnsNoContent()
    {
        var articleId = Guid.NewGuid();

        var result = await _controller.Delete(articleId);

        result.Should().BeOfType<NoContentResult>();
        _mockMediator.Verify(m => m.Send(It.Is<DeleteArticleCommand>(c => c.Id == articleId), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public void GetOptions_ReturnsOkWithAllowHeader()
    {
        var httpContext = new DefaultHttpContext();
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        var result = _controller.GetOptions();

        result.Should().BeOfType<OkResult>();
        httpContext.Response.Headers.Should().ContainKey("Allow")
            .WhoseValue.Should().BeEquivalentTo(new[] { "GET, POST, PUT, DELETE, OPTIONS" });
    }

    [Fact]
    public async Task Publish_ExistingArticle_ReturnsNoContent()
    {
        var articleId = Guid.NewGuid();

        var result = await _controller.Publish(articleId);

        result.Should().BeOfType<NoContentResult>();
        _mockMediator.Verify(m => m.Send(It.Is<PublishArticleCommand>(c => c.Id == articleId), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Unpublish_ExistingArticle_ReturnsNoContent()
    {
        var articleId = Guid.NewGuid();

        var result = await _controller.Unpublish(articleId);

        result.Should().BeOfType<NoContentResult>();
        _mockMediator.Verify(m => m.Send(It.Is<UnpublishArticleCommand>(c => c.Id == articleId), It.IsAny<CancellationToken>()), Times.Once);
    }
} 