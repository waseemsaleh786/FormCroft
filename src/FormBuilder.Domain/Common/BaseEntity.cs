namespace FormBuilder.Domain.Common;

/// <summary>
/// Base type for all persisted entities. Provides identity, audit timestamps
/// and a lightweight domain-event buffer that the persistence layer dispatches
/// after a successful <c>SaveChanges</c>.
/// </summary>
public abstract class BaseEntity
{
    private readonly List<DomainEvent> _domainEvents = new();

    public Guid Id { get; protected set; } = Guid.NewGuid();

    public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; protected set; } = DateTime.UtcNow;

    /// <summary>Domain events raised by this entity, awaiting dispatch.</summary>
    public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void Raise(DomainEvent domainEvent) => _domainEvents.Add(domainEvent);

    public void ClearDomainEvents() => _domainEvents.Clear();

    /// <summary>Stamps the entity as modified. Called by aggregate behaviour methods.</summary>
    protected internal void Touch() => UpdatedAt = DateTime.UtcNow;
}
