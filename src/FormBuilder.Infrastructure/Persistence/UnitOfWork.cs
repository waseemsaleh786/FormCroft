using FormBuilder.Domain.Interfaces;

namespace FormBuilder.Infrastructure.Persistence;

/// <summary>
/// Thin wrapper over <see cref="AppDbContext"/> that exposes commit as the
/// domain-facing <see cref="IUnitOfWork"/>. Domain-event dispatch happens inside
/// the context's SaveChanges override.
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _db;

    public UnitOfWork(AppDbContext db) => _db = db;

    public Task<int> SaveChangesAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);
}
