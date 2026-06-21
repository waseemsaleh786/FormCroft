using FormBuilder.Domain.Entities;

namespace FormBuilder.Domain.Interfaces;

public interface IFormRepository
{
    /// <summary>Returns non-deleted forms, optionally filtered by a name search term.</summary>
    Task<IReadOnlyList<Form>> ListAsync(string? search, CancellationToken ct = default);

    /// <summary>Loads a form header only (no layout tree).</summary>
    Task<Form?> GetAsync(Guid id, CancellationToken ct = default);

    /// <summary>Loads a form with its full page → row → column → field tree.</summary>
    Task<Form?> GetWithLayoutAsync(Guid id, CancellationToken ct = default);

    /// <summary>Loads a published form with its layout by public slug.</summary>
    Task<Form?> GetPublishedBySlugAsync(string slug, CancellationToken ct = default);

    Task<bool> SlugExistsAsync(string slug, Guid? excludeId = null, CancellationToken ct = default);

    Task AddAsync(Form form, CancellationToken ct = default);
    void Remove(Form form);
}
