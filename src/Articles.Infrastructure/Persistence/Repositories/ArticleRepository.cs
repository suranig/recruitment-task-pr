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
    private readonly ApplicationDbContext _context;

    public ArticleRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Article?> GetByIdAsync(ArticleId id, CancellationToken cancellationToken = default)
    {
        return await _context.Articles.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<List<Article>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Articles.ToListAsync(cancellationToken);
    }

    public async Task<List<Article>> GetByTagAsync(TagName tagName, CancellationToken cancellationToken = default)
    {
        return await _context.Articles
            .Include(a => a.Authors)
            .Include(a => a.Tags)
            .Where(a => a.Tags.Any(t => t.Name == tagName))
            .ToListAsync(cancellationToken);
    }

    public async Task<Article> AddAsync(Article article, CancellationToken cancellationToken = default)
    {
        await _context.Articles.AddAsync(article, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return article;
    }

    public async Task UpdateAsync(Article article, CancellationToken cancellationToken = default)
    {
        _context.Articles.Update(article);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Article article, CancellationToken cancellationToken = default)
    {
        _context.Articles.Remove(article);
        await _context.SaveChangesAsync(cancellationToken);
    }
} 