namespace FormBuilder.Domain.Common;

/// <summary>Marker base type for domain events raised by aggregates.</summary>
public abstract record DomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
