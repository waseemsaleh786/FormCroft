using FormBuilder.Domain.Entities;
using FormBuilder.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FormBuilder.Infrastructure.Persistence.Repositories;

public class FieldMappingRepository : IFieldMappingRepository
{
    private readonly AppDbContext _db;

    public FieldMappingRepository(AppDbContext db) => _db = db;

    public async Task<IReadOnlyList<FieldMapping>> GetByFormAsync(Guid formId, CancellationToken ct = default)
        => await _db.FieldMappings.AsNoTracking()
            .Where(m => m.FormId == formId)
            .ToListAsync(ct);

    public async Task AddAsync(FieldMapping mapping, CancellationToken ct = default)
        => await _db.FieldMappings.AddAsync(mapping, ct);

    public void Remove(FieldMapping mapping) => _db.FieldMappings.Remove(mapping);

    public async Task ReplaceForFormAsync(Guid formId, IEnumerable<FieldMapping> mappings, CancellationToken ct = default)
    {
        var existing = await _db.FieldMappings.Where(m => m.FormId == formId).ToListAsync(ct);
        _db.FieldMappings.RemoveRange(existing);
        await _db.FieldMappings.AddRangeAsync(mappings, ct);
    }
}
