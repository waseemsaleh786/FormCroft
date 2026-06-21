using FormBuilder.Domain.Entities;
using FormBuilder.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FormBuilder.Infrastructure.Persistence.Repositories;

public class FormRepository : IFormRepository
{
    private readonly AppDbContext _db;

    public FormRepository(AppDbContext db) => _db = db;

    public async Task<IReadOnlyList<Form>> ListAsync(string? search, CancellationToken ct = default)
    {
        var query = _db.Forms
            .Include(f => f.Pages)
                .ThenInclude(p => p.Rows)
                    .ThenInclude(r => r.Columns)
                        .ThenInclude(c => c.Fields)
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(f => EF.Functions.Like(f.Name, $"%{term}%"));
        }

        return await query.OrderByDescending(f => f.UpdatedAt).ToListAsync(ct);
    }

    public async Task<Form?> GetAsync(Guid id, CancellationToken ct = default)
        => await _db.Forms.FirstOrDefaultAsync(f => f.Id == id, ct);

    public async Task<Form?> GetWithLayoutAsync(Guid id, CancellationToken ct = default)
        => await LayoutQuery().FirstOrDefaultAsync(f => f.Id == id, ct);

    public async Task<Form?> GetPublishedBySlugAsync(string slug, CancellationToken ct = default)
        => await LayoutQuery()
            .AsNoTracking()
            .FirstOrDefaultAsync(f => f.Slug == slug && f.Status == Domain.Enums.FormStatus.Published, ct);

    public async Task<bool> SlugExistsAsync(string slug, Guid? excludeId = null, CancellationToken ct = default)
        => await _db.Forms.IgnoreQueryFilters()
            .AnyAsync(f => f.Slug == slug && (excludeId == null || f.Id != excludeId), ct);

    public async Task AddAsync(Form form, CancellationToken ct = default)
        => await _db.Forms.AddAsync(form, ct);

    public void Remove(Form form) => _db.Forms.Remove(form);

    private IQueryable<Form> LayoutQuery()
        => _db.Forms
            // Top level: page → row → column → fields
            .Include(f => f.Pages).ThenInclude(p => p.Rows).ThenInclude(r => r.Columns).ThenInclude(c => c.Fields)
            // Nesting level 1: column → sub-row → column → fields
            .Include(f => f.Pages).ThenInclude(p => p.Rows).ThenInclude(r => r.Columns)
                .ThenInclude(c => c.SubRows).ThenInclude(sr => sr.Columns).ThenInclude(sc => sc.Fields)
            // Nesting level 2: column → sub-row → column → sub-row → column → fields
            .Include(f => f.Pages).ThenInclude(p => p.Rows).ThenInclude(r => r.Columns)
                .ThenInclude(c => c.SubRows).ThenInclude(sr => sr.Columns)
                    .ThenInclude(sc => sc.SubRows).ThenInclude(sr2 => sr2.Columns).ThenInclude(sc2 => sc2.Fields);
}
