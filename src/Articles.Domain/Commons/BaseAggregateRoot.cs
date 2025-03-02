using Articles.Domain.Events;
using System.ComponentModel.DataAnnotations.Schema;

namespace Articles.Domain.Commons;

public abstract class BaseAggregateRoot<TId> : BaseEntity<TId> where TId : EntityId
{
    [NotMapped]
    private readonly List<DomainEvent> _domainEvents = new();
    
    [NotMapped]
    public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();
    
    protected BaseAggregateRoot() { }
    
    protected BaseAggregateRoot(TId id) : base(id) { }
    
    protected void AddDomainEvent(DomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
    
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
} 