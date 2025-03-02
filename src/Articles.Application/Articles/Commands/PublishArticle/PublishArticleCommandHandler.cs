using Articles.Application.Commons.Exceptions;
using Articles.Domain.Interfaces;
using Articles.Domain.ValueObjects;
using MediatR;

namespace Articles.Application.Articles.Commands.PublishArticle;

public class PublishArticleCommandHandler : IRequestHandler<PublishArticleCommand>
{
    private readonly IArticleRepository _articleRepository;

    public PublishArticleCommandHandler(IArticleRepository articleRepository)
    {
        _articleRepository = articleRepository;
    }

    public async Task Handle(PublishArticleCommand request, CancellationToken cancellationToken)
    {
        var article = await _articleRepository.GetByIdAsync(ArticleId.Create(request.Id), cancellationToken);

        if (article == null)
        {
            throw new NotFoundException("Article", request.Id);
        }

        article.Publish();

        await _articleRepository.UpdateAsync(article, cancellationToken);
    }
} 