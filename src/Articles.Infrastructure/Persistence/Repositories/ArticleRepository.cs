using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Articles.Domain.Aggregates.ArticleAggregate;
using Articles.Domain.Interfaces;
using Articles.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Articles.Infrastructure.Persistence.Repositories;

public class ArticleRepository : IArticleRepository
{
    private readonly ApplicationDbContext _dbContext;

    public ArticleRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Article?> GetByIdAsync(ArticleId id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Articles
            .Include(a => a.Tags)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<(List<Article> Items, int TotalCount)> GetAllAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Articles
            .Include(a => a.Tags)
            .AsQueryable();

        var totalCount = await query.CountAsync(cancellationToken);
        
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<List<Article>> GetByTagAsync(TagName tagName, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Articles
            .Include(a => a.Authors)
            .Include(a => a.Tags)
            .Where(a => a.Tags.Any(t => t.Name == tagName))
            .ToListAsync(cancellationToken);
    }

    public async Task<Article> AddAsync(Article article, CancellationToken cancellationToken = default)
    {
        _dbContext.Articles.Add(article);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return article;
    }

    public async Task UpdateAsync(Article article, CancellationToken cancellationToken = default)
    {
        _dbContext.Articles.Update(article);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(ArticleId id, CancellationToken cancellationToken = default)
    {
        var article = await _dbContext.Articles.FindAsync(new object[] { id }, cancellationToken);
        if (article != null)
        {
            _dbContext.Articles.Remove(article);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
} 