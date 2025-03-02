using MediatR;

namespace Articles.Application.Articles.Commands.UpdateArticle;

public class UpdateArticleCommand : IRequest
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
} 