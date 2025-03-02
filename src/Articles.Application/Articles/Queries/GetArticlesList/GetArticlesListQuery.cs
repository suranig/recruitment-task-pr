using Articles.Application.Commons.Models;
using MediatR;

namespace Articles.Application.Articles.Queries.GetArticlesList;

public class GetArticlesListQuery : IRequest<PaginatedList<ArticleListItemDto>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
} 