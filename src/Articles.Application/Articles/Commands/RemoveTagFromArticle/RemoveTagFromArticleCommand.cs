using MediatR;

namespace Articles.Application.Articles.Commands.RemoveTagFromArticle;

public class RemoveTagFromArticleCommand : IRequest
{
    public Guid ArticleId { get; set; }
    public string TagName { get; set; } = string.Empty;
} 