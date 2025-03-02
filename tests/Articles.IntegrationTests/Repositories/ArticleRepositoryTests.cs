using Articles.Domain.Aggregates.ArticleAggregate;
using Articles.Domain.ValueObjects;
using Articles.Infrastructure.Persistence;
using Articles.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Articles.IntegrationTests.Repositories;

public class ArticleRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ArticleRepository _repository;

    public ArticleRepositoryTests()
    {
        var serviceProvider = new ServiceCollection()
            .AddEntityFrameworkInMemoryDatabase()
            .BuildServiceProvider();

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .UseInternalServiceProvider(serviceProvider)
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
        var articleId1 = ArticleId.CreateUnique();
        var articleId2 = ArticleId.CreateUnique();
        var authorId = AuthorId.CreateUnique();
        
        var article1 = Article.Create(articleId1, "Test Article 1", "Test Content 1", authorId);
        article1.AddTag(TagName.Create("tag 1"));
        article1.AddTag(TagName.Create("common tag"));
        
        var article2 = Article.Create(articleId2, "Test Article 2", "Test Content 2", authorId);
        article2.AddTag(TagName.Create("tag 2"));
        article2.AddTag(TagName.Create("common tag"));
        
        _dbContext.Articles.AddRange(article1, article2);
        await _dbContext.SaveChangesAsync();

        var (result, totalCount) = await _repository.GetAllAsync(1, 10);

        Assert.Equal(2, result.Count);
        Assert.Equal(2, totalCount);
        
        var firstArticle = result.First(a => a.Id == articleId1);
        var secondArticle = result.First(a => a.Id == articleId2);
        
        Assert.Equal(2, firstArticle.Tags.Count);
        Assert.Contains(firstArticle.Tags, t => t.Name.Value == "tag 1");
        Assert.Contains(firstArticle.Tags, t => t.Name.Value == "common tag");
        
        Assert.Equal(2, secondArticle.Tags.Count);
        Assert.Contains(secondArticle.Tags, t => t.Name.Value == "tag 2");
        Assert.Contains(secondArticle.Tags, t => t.Name.Value == "common tag");
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
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();
    }
} 