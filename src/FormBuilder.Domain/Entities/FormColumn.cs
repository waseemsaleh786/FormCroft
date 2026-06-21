using FormBuilder.Domain.Common;
using FormBuilder.Domain.Exceptions;

namespace FormBuilder.Domain.Entities;

/// <summary>
/// A column within a row. <see cref="Width"/> uses the 12-unit bootstrap-style
/// grid (12, 6+6, 4+4+4, 3+3+3+3, 8+4, 4+8). Holds fields and/or nested sub-rows.
/// </summary>
public class FormColumn : BaseEntity
{
    private readonly List<FormField> _fields = new();
    private readonly List<FormRow> _subRows = new();

    private FormColumn() { }

    public FormColumn(int width, int order, Guid? id = null)
    {
        if (width is < 1 or > 12)
            throw new DomainException("Column width must be between 1 and 12.");
        if (id.HasValue && id.Value != Guid.Empty) Id = id.Value;
        Width = width;
        Order = order;
    }

    public Guid RowId { get; private set; }
    public int Width { get; private set; }
    public int Order { get; private set; }

    public IReadOnlyCollection<FormField> Fields => _fields.AsReadOnly();

    /// <summary>Nested rows placed inside this column (supports nested layouts).</summary>
    public IReadOnlyCollection<FormRow> SubRows => _subRows.AsReadOnly();

    public void SetOrder(int order) => Order = order;

    public void AddField(FormField field) => _fields.Add(field);

    public void AddSubRow(FormRow row) => _subRows.Add(row);
}
