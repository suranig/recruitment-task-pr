using Articles.Application.Commons.Exceptions;
using Articles.Domain.Interfaces;
using Articles.Domain.ValueObjects;
using MediatR;

namespace Articles.Application.Articles.Commands.AddTagToArticle;

public class AddTagToArticleCommandHandler : IRequestHandler<AddTagToArticleCommand>
{
    private readonly IArticleRepository _articleRepository;

    public AddTagToArticleCommandHandler(IArticleRepository articleRepository)
    {
        _articleRepository = articleRepository;
    }

    public async Task Handle(AddTagToArticleCommand request, CancellationToken cancellationToken)
    {
        var article = await _articleRepository.GetByIdAsync(ArticleId.Create(request.ArticleId), cancellationToken);

        if (article == null)
        {
            throw new NotFoundException("Article", request.ArticleId);
        }

        article.AddTag(TagName.Create(request.TagName));

        await _articleRepository.UpdateAsync(article, cancellationToken);
    }
} 