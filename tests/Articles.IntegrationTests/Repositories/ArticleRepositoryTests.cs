using Articles.Domain.Aggregates.ArticleAggregate;
using Articles.Domain.ValueObjects;
using Articles.Infrastructure.Persistence;
using Articles.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Articles.IntegrationTests.Repositories;

public class ArticleRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ArticleRepository _repository;

    public ArticleRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _dbContext = new ApplicationDbContext(options);
        _repository = new ArticleRepository(_dbContext);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnArticle_WhenArticleExists()
    {
        var articleId = ArticleId.CreateUnique();
        var authorId = AuthorId.CreateUnique();
        var article = Article.Create(articleId, "Test Article", "Test Content", authorId);
        
        article.AddTag(TagName.Create("tag 1"));
        
        _dbContext.Articles.Add(article);
        await _dbContext.SaveChangesAsync();

        var result = await _repository.GetByIdAsync(article.Id);

        Assert.NotNull(result);
        Assert.Equal(article.Id, result.Id);
        Assert.Equal("Test Article", result.Title);
        Assert.Single(result.Tags);
        Assert.Equal("tag 1", result.Tags.First().Name.Value);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllArticles()
    {
        _dbContext.Articles.RemoveRange(_dbContext.Articles);
        await _dbContext.SaveChangesAsync();

        var authorId = AuthorId.CreateUnique();
        var article1 = Article.Create(ArticleId.CreateUnique(), "Article 1", "Content 1", authorId);
        var article2 = Article.Create(ArticleId.CreateUnique(), "Article 2", "Content 2", authorId);
        
        _dbContext.Articles.AddRange(article1, article2);
        await _dbContext.SaveChangesAsync();

        var result = await _repository.GetAllAsync(1, 10);

        Assert.Equal(2, result.Items.Count);
        Assert.Equal(2, result.TotalCount);
        Assert.Contains(result.Items, a => a.Title == "Article 1");
        Assert.Contains(result.Items, a => a.Title == "Article 2");
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveArticle()
    {
        var articleId = ArticleId.CreateUnique();
        var authorId = AuthorId.CreateUnique();
        var article = Article.Create(articleId, "Test Article", "Test Content", authorId);
        article.AddTag(TagName.Create("tag 1"));
        article.AddTag(TagName.Create("tag 2"));
        
        _dbContext.Articles.Add(article);
        await _dbContext.SaveChangesAsync();

        await _repository.DeleteAsync(article.Id);

        var result = await _dbContext.Articles.FindAsync(article.Id);
        Assert.Null(result);
    }

    public void Dispose()
    {
        _dbContext.Dispose();
    }
} 