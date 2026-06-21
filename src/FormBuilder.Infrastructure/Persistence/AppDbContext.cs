using System.Reflection;
using FormBuilder.Application.Common.Events;
using FormBuilder.Domain.Common;
using FormBuilder.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FormBuilder.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    private readonly IPublisher? _publisher;

    public AppDbContext(DbContextOptions<AppDbContext> options, IPublisher? publisher = null)
        : base(options)
        => _publisher = publisher;

    public DbSet<Form> Forms => Set<Form>();
    public DbSet<FormPage> FormPages => Set<FormPage>();
    public DbSet<FormRow> FormRows => Set<FormRow>();
    public DbSet<FormColumn> FormColumns => Set<FormColumn>();
    public DbSet<FormField> FormFields => Set<FormField>();
    public DbSet<FieldMapping> FieldMappings => Set<FieldMapping>();
    public DbSet<FormSubmission> FormSubmissions => Set<FormSubmission>();
    public DbSet<FormSubmissionValue> FormSubmissionValues => Set<FormSubmissionValue>();
    public DbSet<DatabaseConnection> DatabaseConnections => Set<DatabaseConnection>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Domain events are an in-memory buffer, never persisted.
        modelBuilder.Ignore<DomainEvent>();
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (entityType.ClrType.GetProperty(nameof(BaseEntity.DomainEvents)) is not null)
                modelBuilder.Entity(entityType.ClrType).Ignore(nameof(BaseEntity.DomainEvents));
        }

        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var result = await base.SaveChangesAsync(cancellationToken);
        await DispatchDomainEventsAsync(cancellationToken);
        return result;
    }

    /// <summary>Publishes buffered domain events via MediatR after the transaction commits.</summary>
    private async Task DispatchDomainEventsAsync(CancellationToken ct)
    {
        if (_publisher is null) return;

        var entities = ChangeTracker.Entries<BaseEntity>()
            .Where(e => e.Entity.DomainEvents.Count > 0)
            .Select(e => e.Entity)
            .ToList();

        var events = entities.SelectMany(e => e.DomainEvents).ToList();
        entities.ForEach(e => e.ClearDomainEvents());

        foreach (var domainEvent in events)
        {
            var notificationType = typeof(DomainEventNotification<>).MakeGenericType(domainEvent.GetType());
            var notification = (INotification)Activator.CreateInstance(notificationType, domainEvent)!;
            await _publisher.Publish(notification, ct);
        }
    }
}
