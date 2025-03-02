using MediatR;

namespace Articles.Application.Articles.Queries.GetArticle;

public class GetArticleQuery : IRequest<ArticleDetailsDto?>
{
    public Guid Id { get; set; }
} 