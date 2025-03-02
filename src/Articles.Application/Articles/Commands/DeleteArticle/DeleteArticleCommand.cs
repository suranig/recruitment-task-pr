using MediatR;

namespace Articles.Application.Articles.Commands.DeleteArticle;

public class DeleteArticleCommand : IRequest
{
    public Guid Id { get; set; }
} 