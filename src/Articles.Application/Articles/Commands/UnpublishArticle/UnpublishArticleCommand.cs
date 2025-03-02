using MediatR;

namespace Articles.Application.Articles.Commands.UnpublishArticle;

public class UnpublishArticleCommand : IRequest
{
    public Guid Id { get; set; }
} 