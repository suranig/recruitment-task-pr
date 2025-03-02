using Articles.Application.Articles.Commands.AddTagToArticle;
using Articles.Application.Articles.Commands.CreateArticle;
using Articles.Application.Articles.Commands.DeleteArticle;
using Articles.Application.Articles.Commands.PublishArticle;
using Articles.Application.Articles.Commands.RemoveTagFromArticle;
using Articles.Application.Articles.Commands.UnpublishArticle;
using Articles.Application.Articles.Commands.UpdateArticle;
using Articles.Application.Articles.Queries.GetArticle;
using Articles.Application.Articles.Queries.GetArticlesList;
using Articles.Application.Commons.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Articles.API.Controllers;

[ApiController]
[Route("v1/articles")]
[Produces("application/json")]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public class ArticlesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ArticlesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Retrieves the publsihed article with the specified ID.
    /// </summary>
    /// <param name="id">Article identifier</param>
    /// <returns>Article details</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ArticleDetailsDto>> GetById(Guid id)
    {
        var article = await _mediator.Send(new GetArticleQuery { Id = id });

        if (article == null)
            return NotFound();

        return Ok(article);
    }

    /// <summary>
    /// Retrieves all articles.
    /// </summary>
    /// <param name="query">Filtering and pagination parameters</param>
    /// <returns>Paginated list of articles</returns>
    /// <response code="200">Zwraca listę artykułów</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedList<ArticleListItemDto>>> GetList([FromQuery] GetArticlesListQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Creates a new article.
    /// </summary>
    /// <param name="command">New article data</param>
    /// <returns>Identifier of the created article</returns>
    /// <response code="201">Article created successfully</response>
    /// <response code="400">Invalid input data</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CreateArticleResponse>> Create(CreateArticleCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Updates an existing article.
    /// </summary>
    /// <param name="id">Article identifier</param>
    /// <param name="command">Updated article data</param>
    /// <returns>No content</returns>
    /// <response code="204">Article updated successfully</response>
    /// <response code="400">Invalid input data</response>
    /// <response code="404">Article not found</response>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Update(Guid id, UpdateArticleCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest();
        }

        await _mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Deletes the specified article.
    /// </summary>
    /// <param name="id">Article identifier</param>
    /// <returns>No content</returns>
    /// <response code="204">Article deleted successfully</response>
    /// <response code="404">Article not found</response>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeleteArticleCommand { Id = id });
        return NoContent();
    }

    /// <summary>
    /// Publishes the specified article.
    /// </summary>
    /// <param name="id">Article identifier</param>
    /// <returns>No content</returns>
    /// <response code="204">Article published successfully</response>
    /// <response code="404">Article not found</response>
    [HttpPost("{id:guid}/publish")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Publish(Guid id)
    {
        await _mediator.Send(new PublishArticleCommand { Id = id });
        return NoContent();
    }

    /// <summary>
    /// Unpublishes the specified article.
    /// </summary>
    /// <param name="id">Article identifier</param>
    /// <returns>No content</returns>
    /// <response code="204">Article unpublished successfully</response>
    /// <response code="404">Article not found</response>
    [HttpPost("{id:guid}/unpublish")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Unpublish(Guid id)
    {
        await _mediator.Send(new UnpublishArticleCommand { Id = id });
        return NoContent();
    }

    /// <summary>
    /// Adds a tag to the specified article.
    /// </summary>
    /// <param name="id">Article identifier</param>
    /// <param name="command">Tag data</param>
    /// <returns>No content</returns>
    /// <response code="204">Tag added successfully</response>
    /// <response code="400">Invalid input data</response>
    [HttpPost("{id:guid}/tags")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> AddTag(Guid id, AddTagToArticleCommand command)
    {
        if (id != command.ArticleId)
        {
            return BadRequest();
        }

        await _mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Removes a tag from the specified article.
    /// </summary>
    /// <param name="id">Article identifier</param>
    /// <param name="tagName">Name of the tag</param>
    /// <returns>No content</returns>
    /// <response code="204">Tag removed successfully</response>
    /// <response code="400">Invalid input data</response>
    [HttpDelete("{id:guid}/tags/{tagName}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> RemoveTag(Guid id, string tagName)
    {
        var command = new RemoveTagFromArticleCommand
        {
            ArticleId = id,
            TagName = tagName
        };

        await _mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Returns the list of allowed HTTP methods.
    /// </summary>
    /// <returns>Allowed HTTP methods</returns>
    [HttpOptions]
    public ActionResult GetOptions()
    {
        if (Response != null)
        {
            Response.Headers.Append("Allow", "GET, POST, PUT, DELETE, OPTIONS");
        }
        return Ok();
    }
} 