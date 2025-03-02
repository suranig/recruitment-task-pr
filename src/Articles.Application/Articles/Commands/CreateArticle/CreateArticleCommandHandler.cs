using Articles.Domain.Interfaces;
using Articles.Domain.ValueObjects;
using MediatR;

namespace Articles.Application.Articles.Commands.CreateArticle;

public class CreateArticleCommandHandler : IRequestHandler<CreateArticleCommand, CreateArticleResponse>
{
    private readonly IArticleRepository _articleRepository;

    public CreateArticleCommandHandler(IArticleRepository articleRepository)
    {
        _articleRepository = articleRepository;
    }

    public async Task<CreateArticleResponse> Handle(CreateArticleCommand request, CancellationToken cancellationToken)
    {
        var articleId = ArticleId.CreateUnique();
        var authorId = AuthorId.Create(request.AuthorId);

        var article = Domain.Aggregates.ArticleAggregate.Article.Create(
            articleId,
            request.Title,
            request.Content,
            authorId
        );

        var savedArticle = await _articleRepository.AddAsync(article, cancellationToken);

        return new CreateArticleResponse
        {
            Id = savedArticle.Id.Value,
            Title = savedArticle.Title,
            CreatedAt = savedArticle.CreatedAt
        };
    }
} 