using MediatR;

namespace Articles.Application.Articles.Commands.PublishArticle;

public class PublishArticleCommand : IRequest
{
    public Guid Id { get; set; }
} 