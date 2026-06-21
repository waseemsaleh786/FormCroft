using FormBuilder.Domain.Entities;

namespace FormBuilder.Domain.Interfaces;

public interface IFormSubmissionRepository
{
    Task AddAsync(FormSubmission submission, CancellationToken ct = default);
    Task<FormSubmission?> GetAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<FormSubmission>> GetByFormAsync(Guid formId, CancellationToken ct = default);
    Task<int> CountByFormAsync(Guid formId, CancellationToken ct = default);
}
