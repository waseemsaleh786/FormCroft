using FormBuilder.Domain.Common;

namespace FormBuilder.Domain.Entities;

/// <summary>A page within a form. Pages are ordered and contain layout rows.</summary>
public class FormPage : BaseEntity
{
    private readonly List<FormRow> _rows = new();

    private FormPage() { }

    public FormPage(string name, int order, Guid? id = null)
    {
        if (id.HasValue && id.Value != Guid.Empty) Id = id.Value;
        Name = string.IsNullOrWhiteSpace(name) ? "Untitled Page" : name.Trim();
        Order = order;
    }

    public Guid FormId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public int Order { get; private set; }

    public IReadOnlyCollection<FormRow> Rows => _rows.AsReadOnly();

    public void Rename(string name) => Name = string.IsNullOrWhiteSpace(name) ? Name : name.Trim();
    public void SetOrder(int order) => Order = order;

    public FormRow AddRow(int order)
    {
        var row = new FormRow(order);
        _rows.Add(row);
        return row;
    }

    public void AddRow(FormRow row) => _rows.Add(row);
}
