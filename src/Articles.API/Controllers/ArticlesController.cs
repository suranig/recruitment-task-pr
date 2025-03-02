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
[Route("v1/[controller]")]
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
    /// Pobiera artykuł o podanym ID
    /// </summary>
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
    /// Pobiera listę wszystkich artykułów
    /// </summary>
    /// <param name="query">Parametry filtrowania i paginacji</param>
    /// <returns>Lista artykułów</returns>
    /// <response code="200">Zwraca listę artykułów</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedList<ArticleListItemDto>>> GetList([FromQuery] GetArticlesListQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Tworzy nowy artykuł
    /// </summary>
    /// <param name="command">Dane nowego artykułu</param>
    /// <returns>Identyfikator utworzonego artykułu</returns>
    /// <response code="201">Artykuł został utworzony</response>
    /// <response code="400">Nieprawidłowe dane wejściowe</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CreateArticleResponse>> Create(CreateArticleCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Aktualizuje istniejący artykuł
    /// </summary>
    /// <param name="id">Identyfikator artykułu</param>
    /// <param name="command">Dane do aktualizacji</param>
    /// <returns>Brak zawartości</returns>
    /// <response code="204">Artykuł został zaktualizowany</response>
    /// <response code="400">Nieprawidłowe dane wejściowe</response>
    /// <response code="404">Artykuł nie został znaleziony</response>
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
    /// Usuwa artykuł
    /// </summary>
    /// <param name="id">Identyfikator artykułu</param>
    /// <returns>Brak zawartości</returns>
    /// <response code="204">Artykuł został usunięty</response>
    /// <response code="404">Artykuł nie został znaleziony</response>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeleteArticleCommand { Id = id });
        return NoContent();
    }

    /// <summary>
    /// Publikuje artykuł
    /// </summary>
    /// <param name="id">Identyfikator artykułu</param>
    /// <returns>Brak zawartości</returns>
    /// <response code="204">Artykuł został opublikowany</response>
    /// <response code="404">Artykuł nie został znaleziony</response>
    [HttpPost("{id:guid}/publish")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Publish(Guid id)
    {
        await _mediator.Send(new PublishArticleCommand { Id = id });
        return NoContent();
    }

    /// <summary>
    /// Usuwa artykuł
    /// </summary>
    /// <param name="id">Identyfikator artykułu</param>
    /// <returns>Brak zawartości</returns>
    /// <response code="204">Artykuł został usunięty</response>
    /// <response code="404">Artykuł nie został znaleziony</response>
    [HttpPost("{id:guid}/unpublish")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Unpublish(Guid id)
    {
        await _mediator.Send(new UnpublishArticleCommand { Id = id });
        return NoContent();
    }

    /// <summary>
    /// Dodaje tag do artykułu
    /// </summary>
    /// <param name="id">Identyfikator artykułu</param>
    /// <param name="command">Dane tagu</param>
    /// <returns>Brak zawartości</returns>
    /// <response code="204">Tag został dodany</response>
    /// <response code="400">Nieprawidłowe dane wejściowe</response>
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
    /// Usuwa tag z artykułu
    /// </summary>
    /// <param name="id">Identyfikator artykułu</param>
    /// <param name="tagName">Nazwa tagu</param>
    /// <returns>Brak zawartości</returns>
    /// <response code="204">Tag został usunięty</response>
    /// <response code="400">Nieprawidłowe dane wejściowe</response>
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