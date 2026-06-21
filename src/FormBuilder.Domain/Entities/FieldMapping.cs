using FormBuilder.Domain.Common;
using FormBuilder.Domain.Exceptions;

namespace FormBuilder.Domain.Entities;

/// <summary>
/// Maps a single form field to a database table/column. The invariant
/// "one form field → one column" is enforced at the repository/handler level
/// via a unique index on (FormId, FormFieldId).
/// </summary>
public class FieldMapping : BaseEntity
{
    private FieldMapping() { }

    public FieldMapping(Guid formId, Guid formFieldId, string tableName, string columnName)
    {
        if (formId == Guid.Empty) throw new DomainException("FormId is required.");
        if (formFieldId == Guid.Empty) throw new DomainException("FormFieldId is required.");
        if (string.IsNullOrWhiteSpace(tableName)) throw new DomainException("Table name is required.");
        if (string.IsNullOrWhiteSpace(columnName)) throw new DomainException("Column name is required.");

        FormId = formId;
        FormFieldId = formFieldId;
        TableName = tableName.Trim();
        ColumnName = columnName.Trim();
    }

    public Guid FormId { get; private set; }
    public Guid FormFieldId { get; private set; }
    public string TableName { get; private set; } = string.Empty;
    public string ColumnName { get; private set; } = string.Empty;
}
