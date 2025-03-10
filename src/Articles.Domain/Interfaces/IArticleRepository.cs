using Articles.Domain.Aggregates.ArticleAggregate;
using Articles.Domain.ValueObjects;

namespace Articles.Domain.Interfaces;

public interface IArticleRepository
{
    Task<Article?> GetByIdAsync(ArticleId id, CancellationToken cancellationToken = default);
    Task<(List<Article> Items, int TotalCount)> GetAllAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<List<Article>> GetByTagAsync(TagName tagName, CancellationToken cancellationToken = default);
    Task<Article> AddAsync(Article article, CancellationToken cancellationToken = default);
    Task UpdateAsync(Article article, CancellationToken cancellationToken = default);
    Task DeleteAsync(ArticleId id, CancellationToken cancellationToken = default);
} 