using Articles.Domain.Events;

namespace Articles.Application.Commons.Interfaces;

public interface IDomainEventService
{
    Task Publish(DomainEvent domainEvent, CancellationToken cancellationToken = default);
} 