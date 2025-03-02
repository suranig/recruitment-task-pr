using Articles.Application.Commons.Exceptions;
using Articles.Domain.Interfaces;
using Articles.Domain.ValueObjects;
using MediatR;

namespace Articles.Application.Articles.Commands.UpdateArticle;

public class UpdateArticleCommandHandler : IRequestHandler<UpdateArticleCommand>
{
    private readonly IArticleRepository _articleRepository;

    public UpdateArticleCommandHandler(IArticleRepository articleRepository)
    {
        _articleRepository = articleRepository;
    }

    public async Task Handle(UpdateArticleCommand request, CancellationToken cancellationToken)
    {
        var article = await _articleRepository.GetByIdAsync(ArticleId.Create(request.Id), cancellationToken);

        if (article == null)
        {
            throw new NotFoundException("Article", request.Id);
        }

        article.UpdateTitle(request.Title);
        article.UpdateContent(request.Content);

        await _articleRepository.UpdateAsync(article, cancellationToken);
    }
} 