using FormBuilder.Domain.Entities;
using FormBuilder.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FormBuilder.Infrastructure.Persistence.Repositories;

public class FormSubmissionRepository : IFormSubmissionRepository
{
    private readonly AppDbContext _db;

    public FormSubmissionRepository(AppDbContext db) => _db = db;

    public async Task AddAsync(FormSubmission submission, CancellationToken ct = default)
        => await _db.FormSubmissions.AddAsync(submission, ct);

    public async Task<FormSubmission?> GetAsync(Guid id, CancellationToken ct = default)
        => await _db.FormSubmissions
            .Include(s => s.Values)
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id, ct);

    public async Task<IReadOnlyList<FormSubmission>> GetByFormAsync(Guid formId, CancellationToken ct = default)
        => await _db.FormSubmissions
            .Include(s => s.Values)
            .AsNoTracking()
            .Where(s => s.FormId == formId)
            .OrderByDescending(s => s.SubmittedAt)
            .ToListAsync(ct);

    public async Task<int> CountByFormAsync(Guid formId, CancellationToken ct = default)
        => await _db.FormSubmissions.CountAsync(s => s.FormId == formId, ct);
}
