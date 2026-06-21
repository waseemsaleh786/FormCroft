using FormBuilder.Domain.Common;

namespace FormBuilder.Domain.Entities;

/// <summary>
/// A horizontal row holding one or more columns. A row is either top-level
/// (owned by a <see cref="FormPage"/> via <see cref="PageId"/>) or a nested
/// sub-row (owned by a <see cref="FormColumn"/> via <see cref="ParentColumnId"/>).
/// </summary>
public class FormRow : BaseEntity
{
    private readonly List<FormColumn> _columns = new();

    private FormRow() { }

    public FormRow(int order, Guid? id = null)
    {
        if (id.HasValue && id.Value != Guid.Empty) Id = id.Value;
        Order = order;
    }

    /// <summary>Set when this is a top-level row on a page; null for nested sub-rows.</summary>
    public Guid? PageId { get; private set; }

    /// <summary>Set when this is a nested sub-row inside a column; null for top-level rows.</summary>
    public Guid? ParentColumnId { get; private set; }

    public int Order { get; private set; }

    public IReadOnlyCollection<FormColumn> Columns => _columns.AsReadOnly();

    public void SetOrder(int order) => Order = order;

    /// <summary>Adds a column with the given 12-grid width (e.g. 12, 6, 4, 3, 8).</summary>
    public FormColumn AddColumn(int width, int order)
    {
        var column = new FormColumn(width, order);
        _columns.Add(column);
        return column;
    }

    public void AddColumn(FormColumn column) => _columns.Add(column);
}
