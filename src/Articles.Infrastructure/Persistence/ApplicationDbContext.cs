using Articles.Application.Commons.Interfaces;
using Articles.Domain.Aggregates.ArticleAggregate;
using Articles.Domain.Commons;
using Articles.Domain.Events;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Articles.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    private readonly IDomainEventService? _domainEventService;
    
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, 
        IDomainEventService? domainEventService = null) : base(options)
    {
        _domainEventService = domainEventService;
    }
    
    public DbSet<Article> Articles => Set<Article>();
    public DbSet<ArticleAuthor> ArticleAuthors => Set<ArticleAuthor>();
    public DbSet<ArticleTag> ArticleTags => Set<ArticleTag>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Ignore<DomainEvent>();
        
        modelBuilder.Entity<ArticleTag>()
            .HasOne(at => at.Article)
            .WithMany("Tags")
            .HasForeignKey("ArticleId");
        
        modelBuilder.Entity<ArticleAuthor>()
            .HasOne(aa => aa.Article)
            .WithMany("Authors")
            .HasForeignKey("ArticleId");
    }
    
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var result = await base.SaveChangesAsync(cancellationToken);
        
        if (_domainEventService != null)
        {
            await DispatchEvents(cancellationToken);
        }
        
        return result;
    }
    
    private async Task DispatchEvents(CancellationToken cancellationToken)
    {
        var aggregateRoots = ChangeTracker.Entries()
            .Where(e => IsAggregateRoot(e.Entity.GetType()))
            .Select(e => e.Entity)
            .ToList();
        
        var domainEvents = aggregateRoots
            .SelectMany(GetDomainEvents)
            .OrderBy(e => e.OccurredOn)
            .ToList();
        
        foreach (var aggregateRoot in aggregateRoots)
        {
            ClearDomainEvents(aggregateRoot);
        }
        
        foreach (var domainEvent in domainEvents)
        {
            await _domainEventService!.Publish(domainEvent, cancellationToken);
        }
    }
    
    private bool IsAggregateRoot(Type type)
    {
        while (type != null && type != typeof(object))
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(BaseAggregateRoot<>))
                return true;
            
            type = type.BaseType!;
        }
        
        return false;
    }
    
    private IEnumerable<DomainEvent> GetDomainEvents(object aggregateRoot)
    {
        var property = aggregateRoot.GetType().GetProperty("DomainEvents");
        if (property != null)
        {
            var events = property.GetValue(aggregateRoot) as IEnumerable<DomainEvent>;
            if (events != null)
                return events;
        }
        
        return Enumerable.Empty<DomainEvent>();
    }
    
    private void ClearDomainEvents(object aggregateRoot)
    {
        var method = aggregateRoot.GetType().GetMethod("ClearDomainEvents");
        if (method != null)
        {
            method.Invoke(aggregateRoot, null);
        }
    }
} 