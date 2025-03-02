using Articles.Domain.Aggregates.ArticleAggregate;
using Articles.Domain.ValueObjects;
using Articles.Infrastructure.Persistence;
using Articles.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Articles.IntegrationTests.Repositories;

public class ArticleRepositoryTests
{
    [Fact]
    public async Task GetByIdAsync_ShouldReturnArticle_WhenArticleExists()
    {
        var dbContext = CreateDbContext();
        var repository = new ArticleRepository(dbContext);
        var article = CreateTestArticle();
        await repository.AddAsync(article);

        var result = await repository.GetByIdAsync(article.Id);

        Assert.NotNull(result);
        Assert.Equal(article.Id, result.Id);
        Assert.Equal(article.Title, result.Title);
        Assert.Equal(article.Content, result.Content);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenArticleDoesNotExist()
    {
        var dbContext = CreateDbContext();
        var repository = new ArticleRepository(dbContext);
        var nonExistentId = ArticleId.CreateUnique();

        var result = await repository.GetByIdAsync(nonExistentId);

        Assert.Null(result);
    }

    [Fact]
    public async Task AddAsync_ShouldAddArticle()
    {
        var dbContext = CreateDbContext();
        var repository = new ArticleRepository(dbContext);
        var article = CreateTestArticle();

        await repository.AddAsync(article);

        var result = await dbContext.Articles.FirstOrDefaultAsync(a => a.Id == article.Id);
        Assert.NotNull(result);
        Assert.Equal(article.Id, result.Id);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateArticle()
    {
        var dbContext = CreateDbContext();
        var repository = new ArticleRepository(dbContext);
        var article = CreateTestArticle();
        await repository.AddAsync(article);

        article.UpdateTitle("Updated Title");
        await repository.UpdateAsync(article);

        var result = await dbContext.Articles.FirstOrDefaultAsync(a => a.Id == article.Id);
        Assert.NotNull(result);
        Assert.Equal("Updated Title", result.Title);
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteArticle()
    {
        var dbContext = CreateDbContext();
        var repository = new ArticleRepository(dbContext);
        var article = CreateTestArticle();
        await repository.AddAsync(article);

        await repository.DeleteAsync(article);

        var result = await dbContext.Articles.FirstOrDefaultAsync(a => a.Id == article.Id);
        Assert.Null(result);
    }

    private ApplicationDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }

    private Article CreateTestArticle()
    {
        var articleId = ArticleId.CreateUnique();
        var title = "Test Article";
        var content = "Test Content";
        var authorId = AuthorId.CreateUnique();

        return Article.Create(articleId, title, content, authorId);
    }
} 