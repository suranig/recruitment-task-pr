using MediatR;

namespace Articles.Application.Articles.Commands.CreateArticle;

public class CreateArticleCommand : IRequest<CreateArticleResponse>
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public Guid AuthorId { get; set; }
}

public class CreateArticleResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
} 