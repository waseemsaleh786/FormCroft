using FormBuilder.Domain.Entities;

namespace FormBuilder.Domain.Interfaces;

public interface IFieldMappingRepository
{
    Task<IReadOnlyList<FieldMapping>> GetByFormAsync(Guid formId, CancellationToken ct = default);
    Task AddAsync(FieldMapping mapping, CancellationToken ct = default);
    void Remove(FieldMapping mapping);

    /// <summary>Replaces all mappings for a form in one operation (used by Save).</summary>
    Task ReplaceForFormAsync(Guid formId, IEnumerable<FieldMapping> mappings, CancellationToken ct = default);
}
