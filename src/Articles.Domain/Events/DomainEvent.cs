namespace Articles.Domain.Events;
using System.ComponentModel.DataAnnotations.Schema;

[NotMapped]
public abstract class DomainEvent
{
    public DateTime OccurredOn { get; }
    
    protected DomainEvent()
    {
        OccurredOn = DateTime.UtcNow;
    }
} 