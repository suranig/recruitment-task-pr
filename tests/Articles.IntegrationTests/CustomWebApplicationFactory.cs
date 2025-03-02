using Articles.API;
using Articles.Domain.Aggregates.ArticleAggregate;
using Articles.Domain.Interfaces;
using Articles.Domain.ValueObjects;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Articles.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly List<Article> _articles = new();
    public readonly Mock<IArticleRepository> _mockArticleRepository;

    public CustomWebApplicationFactory()
    {
        _mockArticleRepository = new Mock<IArticleRepository>();
        SetupMockRepository();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(IArticleRepository));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            services.AddScoped(_ => _mockArticleRepository.Object);
        });
    }

    private void SetupMockRepository()
    {
        _mockArticleRepository.Setup(r => r.GetByIdAsync(It.IsAny<ArticleId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ArticleId id, CancellationToken _) => 
                _articles.FirstOrDefault(a => a.Id.Value == id.Value));

        _mockArticleRepository.Setup(r => r.GetAllAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((int pageNumber, int pageSize, CancellationToken _) => 
            {
                var items = _articles.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                return (Items: items, TotalCount: _articles.Count);
            });

        _mockArticleRepository.Setup(r => r.AddAsync(It.IsAny<Article>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Article article, CancellationToken _) =>
            {
                _articles.Add(article);
                return article;
            });

        _mockArticleRepository.Setup(r => r.UpdateAsync(It.IsAny<Article>(), It.IsAny<CancellationToken>()))
            .Returns((Article article, CancellationToken _) =>
            {
                var index = _articles.FindIndex(a => a.Id.Value == article.Id.Value);
                if (index >= 0)
                {
                    _articles[index] = article;
                }
                return Task.CompletedTask;
            });

        _mockArticleRepository.Setup(r => r.DeleteAsync(It.IsAny<ArticleId>(), It.IsAny<CancellationToken>()))
            .Returns((ArticleId id, CancellationToken _) =>
            {
                var article = _articles.FirstOrDefault(a => a.Id.Value == id.Value);
                if (article != null)
                {
                    _articles.Remove(article);
                }
                return Task.CompletedTask;
            });

        _mockArticleRepository.Setup(r => r.GetByTagAsync(It.IsAny<TagName>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TagName tagName, CancellationToken _) =>
            {
                return _articles.Where(a => a.Tags.Any(t => t.Name.Value == tagName.Value)).ToList();
            });
    }
} 