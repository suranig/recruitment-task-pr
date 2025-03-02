using Articles.Application.Commons.Interfaces;
using Articles.Domain.Interfaces;
using Articles.Infrastructure.Persistence;
using Articles.Infrastructure.Persistence.Repositories;
using Articles.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Articles.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

        services.AddScoped<IArticleRepository, ArticleRepository>();
        services.AddScoped<IDomainEventService, DomainEventService>();

        return services;
    }
} 