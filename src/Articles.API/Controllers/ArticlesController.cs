using Articles.Application.Articles.Commands.CreateArticle;
using Articles.Application.Articles.Commands.DeleteArticle;
using Articles.Application.Articles.Commands.UpdateArticle;
using Articles.Application.Articles.Queries.GetArticle;
using Articles.Application.Articles.Queries.GetArticlesList;
using Articles.Application.Commons.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Articles.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ArticlesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ArticlesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ArticleDetailsDto>> GetById(Guid id)
    {
        var article = await _mediator.Send(new GetArticleQuery { Id = id });

        if (article == null)
            return NotFound();

        return Ok(article);
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedList<ArticleListItemDto>>> GetList([FromQuery] GetArticlesListQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<CreateArticleResponse>> Create(CreateArticleCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult> Update(Guid id, UpdateArticleCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest();
        }

        await _mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeleteArticleCommand { Id = id });
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