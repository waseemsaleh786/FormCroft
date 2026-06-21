namespace FormBuilder.Domain.Interfaces;

/// <summary>Commits the current transaction and dispatches buffered domain events.</summary>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
