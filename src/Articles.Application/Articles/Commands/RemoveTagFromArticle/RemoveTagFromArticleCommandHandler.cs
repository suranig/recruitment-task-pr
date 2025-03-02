using Articles.Application.Commons.Exceptions;
using Articles.Domain.Interfaces;
using Articles.Domain.ValueObjects;
using MediatR;

namespace Articles.Application.Articles.Commands.RemoveTagFromArticle;

public class RemoveTagFromArticleCommandHandler : IRequestHandler<RemoveTagFromArticleCommand>
{
    private readonly IArticleRepository _articleRepository;

    public RemoveTagFromArticleCommandHandler(IArticleRepository articleRepository)
    {
        _articleRepository = articleRepository;
    }

    public async Task Handle(RemoveTagFromArticleCommand request, CancellationToken cancellationToken)
    {
        var article = await _articleRepository.GetByIdAsync(ArticleId.Create(request.ArticleId), cancellationToken);

        if (article == null)
        {
            throw new NotFoundException("Article", request.ArticleId);
        }

        article.RemoveTag(TagName.Create(request.TagName));

        await _articleRepository.UpdateAsync(article, cancellationToken);
    }
} 