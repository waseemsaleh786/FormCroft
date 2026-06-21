using FormBuilder.Domain.Common;
using MediatR;

namespace FormBuilder.Application.Common.Events;

/// <summary>
/// Adapts a domain <see cref="DomainEvent"/> to a MediatR <see cref="INotification"/>
/// so handlers can subscribe to specific event types. Published by the persistence
/// layer after a successful SaveChanges.
/// </summary>
public sealed record DomainEventNotification<TDomainEvent>(TDomainEvent DomainEvent) : INotification
    where TDomainEvent : DomainEvent;
