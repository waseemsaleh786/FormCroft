using FormBuilder.Domain.Entities;

namespace FormBuilder.Domain.Interfaces;

/// <summary>
/// Reads table/column metadata from a target database using INFORMATION_SCHEMA.
/// Powers the right-hand panel of the Field Mapping screen.
/// </summary>
public interface ITableMetadataService
{
    Task<IReadOnlyList<DatabaseTable>> GetTablesAsync(string? connectionString = null, CancellationToken ct = default);

    Task<IReadOnlyList<DatabaseColumn>> GetColumnsAsync(string tableName, string? connectionString = null, CancellationToken ct = default);

    /// <summary>Returns true if the column exists on the table (mapping validation).</summary>
    Task<bool> ColumnExistsAsync(string tableName, string columnName, string? connectionString = null, CancellationToken ct = default);
}
