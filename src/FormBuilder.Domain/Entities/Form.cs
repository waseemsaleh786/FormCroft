using FormBuilder.Domain.Common;
using FormBuilder.Domain.Enums;
using FormBuilder.Domain.Events;
using FormBuilder.Domain.Exceptions;

namespace FormBuilder.Domain.Entities;

/// <summary>
/// Aggregate root for the form-builder. Owns the page → row → column → field
/// layout tree and enforces the publishing lifecycle.
/// </summary>
public class Form : BaseEntity
{
    private readonly List<FormPage> _pages = new();

    // EF Core constructor
    private Form() { }

    private Form(string name, string? description, string slug)
    {
        Name = name;
        Description = description;
        Slug = slug;
        Status = FormStatus.Draft;
    }

    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }

    /// <summary>URL-friendly identifier used by the public runtime route /forms/{slug}.</summary>
    public string Slug { get; private set; } = string.Empty;

    public FormStatus Status { get; private set; }

    /// <summary>Soft-delete flag. Deleted forms are filtered out of all queries.</summary>
    public bool IsDeleted { get; private set; }

    public IReadOnlyCollection<FormPage> Pages => _pages.AsReadOnly();

    public static Form Create(string name, string? description, string slug)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Form name is required.");

        var form = new Form(name.Trim(), description?.Trim(), slug);
        // A new form starts with a single empty page, matching the designer mockup.
        form._pages.Add(new FormPage("Page 1", 0));
        form.Raise(new FormCreatedEvent(form.Id, form.Name));
        return form;
    }

    public void Rename(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Form name is required.");
        EnsureEditable();
        Name = name.Trim();
        Touch();
    }

    public void UpdateDescription(string? description)
    {
        EnsureEditable();
        Description = description?.Trim();
        Touch();
    }

    public void SetSlug(string slug)
    {
        Slug = slug;
        Touch();
    }

    public FormPage AddPage(string name)
    {
        EnsureEditable();
        var page = new FormPage(name, _pages.Count);
        _pages.Add(page);
        Touch();
        return page;
    }

    public void RemovePage(Guid pageId)
    {
        EnsureEditable();
        var page = _pages.FirstOrDefault(p => p.Id == pageId)
                   ?? throw new DomainException("Page not found.");
        if (_pages.Count == 1)
            throw new DomainException("A form must have at least one page.");
        _pages.Remove(page);
        Reorder();
        Touch();
    }

    /// <summary>Replaces the entire page collection. Used when the designer saves a layout.</summary>
    public void ReplacePages(IEnumerable<FormPage> pages)
    {
        EnsureEditable();
        _pages.Clear();
        var order = 0;
        foreach (var page in pages)
        {
            page.SetOrder(order++);
            _pages.Add(page);
        }
        Touch();
    }

    public void Publish()
    {
        if (Status == FormStatus.Published) return;
        if (Status == FormStatus.Archived)
            throw new DomainException("Archived forms cannot be published.");
        Status = FormStatus.Published;
        Touch();
        Raise(new FormPublishedEvent(Id, Name));
    }

    public void Archive()
    {
        if (Status != FormStatus.Published)
            throw new DomainException("Only published forms can be archived.");
        Status = FormStatus.Archived;
        Touch();
        Raise(new FormArchivedEvent(Id));
    }

    public void RevertToDraft()
    {
        if (Status == FormStatus.Archived)
            throw new DomainException("Archived forms cannot be edited.");
        Status = FormStatus.Draft;
        Touch();
    }

    public void SoftDelete()
    {
        IsDeleted = true;
        Touch();
    }

    /// <summary>Published forms are read-only; editing requires a draft or archived-revert.</summary>
    private void EnsureEditable()
    {
        if (Status == FormStatus.Published)
            throw new DomainException("Published forms are read-only. Revert to draft to edit.");
        if (Status == FormStatus.Archived)
            throw new DomainException("Archived forms cannot be edited.");
    }

    private void Reorder()
    {
        var order = 0;
        foreach (var page in _pages.OrderBy(p => p.Order))
            page.SetOrder(order++);
    }
}
