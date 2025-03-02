using Articles.Application.Commons.Exceptions;
using Articles.Domain.Interfaces;
using Articles.Domain.ValueObjects;
using MediatR;

namespace Articles.Application.Articles.Commands.UnpublishArticle;

public class UnpublishArticleCommandHandler : IRequestHandler<UnpublishArticleCommand>
{
    private readonly IArticleRepository _articleRepository;

    public UnpublishArticleCommandHandler(IArticleRepository articleRepository)
    {
        _articleRepository = articleRepository;
    }

    public async Task Handle(UnpublishArticleCommand request, CancellationToken cancellationToken)
    {
        var article = await _articleRepository.GetByIdAsync(ArticleId.Create(request.Id), cancellationToken);

        if (article == null)
        {
            throw new NotFoundException("Article", request.Id);
        }

        article.Unpublish();

        await _articleRepository.UpdateAsync(article, cancellationToken);
    }
} 