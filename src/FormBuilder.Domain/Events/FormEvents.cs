using FormBuilder.Domain.Common;

namespace FormBuilder.Domain.Events;

public sealed record FormCreatedEvent(Guid FormId, string Name) : DomainEvent;

public sealed record FormPublishedEvent(Guid FormId, string Name) : DomainEvent;

public sealed record FormArchivedEvent(Guid FormId) : DomainEvent;

public sealed record FormSubmittedEvent(Guid FormId, Guid SubmissionId) : DomainEvent;
