using Articles.Application.Commons.Models;
using Articles.Domain.Interfaces;
using MediatR;

namespace Articles.Application.Articles.Queries.GetArticlesList;

public class GetArticlesListQueryHandler : IRequestHandler<GetArticlesListQuery, PaginatedList<ArticleListItemDto>>
{
    private readonly IArticleRepository _articleRepository;

    public GetArticlesListQueryHandler(IArticleRepository articleRepository)
    {
        _articleRepository = articleRepository;
    }

    public async Task<PaginatedList<ArticleListItemDto>> Handle(GetArticlesListQuery request, CancellationToken cancellationToken)
    {
        var (articles, totalCount) = await _articleRepository.GetAllAsync(
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        var items = articles.Select(a => new ArticleListItemDto
        {
            Id = a.Id.Value,
            Title = a.Title,
            Status = a.Status.ToString(),
            CreatedAt = a.CreatedAt,
            PublishedAt = a.PublishedAt,
            Tags = a.Tags.Select(t => t.Name.Value).ToList()
        }).ToList();

        return new PaginatedList<ArticleListItemDto>(items, totalCount, request.PageNumber, request.PageSize);
    }
} 