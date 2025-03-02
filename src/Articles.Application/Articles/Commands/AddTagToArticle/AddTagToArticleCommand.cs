using MediatR;

namespace Articles.Application.Articles.Commands.AddTagToArticle;

public class AddTagToArticleCommand : IRequest
{
    public Guid ArticleId { get; set; }
    public string TagName { get; set; } = string.Empty;
} 