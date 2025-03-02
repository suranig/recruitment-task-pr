using Articles.Domain.Interfaces;
using Articles.Domain.ValueObjects;
using MediatR;

namespace Articles.Application.Articles.Queries.GetArticle;

public class GetArticleQueryHandler : IRequestHandler<GetArticleQuery, ArticleDetailsDto?>
{
    private readonly IArticleRepository _articleRepository;

    public GetArticleQueryHandler(IArticleRepository articleRepository)
    {
        _articleRepository = articleRepository;
    }

    public async Task<ArticleDetailsDto?> Handle(GetArticleQuery request, CancellationToken cancellationToken)
    {
        var article = await _articleRepository.GetByIdAsync(ArticleId.Create(request.Id), cancellationToken);

        if (article == null)
            return null;

        return new ArticleDetailsDto
        {
            Id = article.Id.Value,
            Title = article.Title,
            Content = article.Content,
            Status = article.Status.ToString(),
            CreatedAt = article.CreatedAt,
            PublishedAt = article.PublishedAt,
            Tags = article.Tags.Select(t => t.Name.Value).ToList(),
            AuthorIds = article.Authors.Select(a => a.AuthorId.Value).ToList()
        };
    }
} 