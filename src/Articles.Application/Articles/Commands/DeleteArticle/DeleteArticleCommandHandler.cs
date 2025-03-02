using Articles.Application.Commons.Exceptions;
using Articles.Domain.Interfaces;
using Articles.Domain.ValueObjects;
using MediatR;

namespace Articles.Application.Articles.Commands.DeleteArticle;

public class DeleteArticleCommandHandler : IRequestHandler<DeleteArticleCommand>
{
    private readonly IArticleRepository _articleRepository;

    public DeleteArticleCommandHandler(IArticleRepository articleRepository)
    {
        _articleRepository = articleRepository;
    }

    public async Task Handle(DeleteArticleCommand request, CancellationToken cancellationToken)
    {
        var articleId = ArticleId.Create(request.Id);
        var article = await _articleRepository.GetByIdAsync(articleId, cancellationToken);

        if (article == null)
        {
            throw new NotFoundException("Article", request.Id);
        }

        await _articleRepository.DeleteAsync(articleId, cancellationToken);
    }
} 