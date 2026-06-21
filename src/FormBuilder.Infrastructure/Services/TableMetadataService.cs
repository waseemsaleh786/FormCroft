using FormBuilder.Domain.Entities;
using FormBuilder.Domain.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace FormBuilder.Infrastructure.Services;

/// <summary>
/// Reads table/column metadata from a SQL Server database via INFORMATION_SCHEMA.
/// When no connection string is supplied it uses the application's default
/// connection (the same LocalDB that holds the seeded sample target tables).
/// FormBuilder's own system tables are hidden so only mappable business tables show.
/// </summary>
public class TableMetadataService : ITableMetadataService
{
    private static readonly HashSet<string> SystemTables = new(StringComparer.OrdinalIgnoreCase)
    {
        "Forms", "FormPages", "FormRows", "FormColumns", "FormFields",
        "FieldMappings", "FormSubmissions", "FormSubmissionValues",
        "DatabaseConnections", "__EFMigrationsHistory"
    };

    private readonly string _defaultConnectionString;

    public TableMetadataService(IConfiguration configuration)
        => _defaultConnectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("DefaultConnection is not configured.");

    public async Task<IReadOnlyList<DatabaseTable>> GetTablesAsync(
        string? connectionString = null, CancellationToken ct = default)
    {
        const string sql = @"
            SELECT TABLE_SCHEMA, TABLE_NAME
            FROM INFORMATION_SCHEMA.TABLES
            WHERE TABLE_TYPE = 'BASE TABLE'
            ORDER BY TABLE_SCHEMA, TABLE_NAME;";

        var tables = new List<DatabaseTable>();
        await using var conn = new SqlConnection(connectionString ?? _defaultConnectionString);
        await conn.OpenAsync(ct);
        await using var cmd = new SqlCommand(sql, conn);
        await using var reader = await cmd.ExecuteReaderAsync(ct);
        while (await reader.ReadAsync(ct))
        {
            var schema = reader.GetString(0);
            var name = reader.GetString(1);
            if (SystemTables.Contains(name)) continue;
            tables.Add(new DatabaseTable(schema, name));
        }
        return tables;
    }

    public async Task<IReadOnlyList<DatabaseColumn>> GetColumnsAsync(
        string tableName, string? connectionString = null, CancellationToken ct = default)
    {
        // Strip optional schema prefix (e.g. "dbo.Customers").
        var (schema, name) = SplitTableName(tableName);

        const string sql = @"
            SELECT
                c.COLUMN_NAME,
                c.DATA_TYPE,
                c.IS_NULLABLE,
                c.CHARACTER_MAXIMUM_LENGTH,
                CASE WHEN pk.COLUMN_NAME IS NOT NULL THEN 1 ELSE 0 END AS IsPk
            FROM INFORMATION_SCHEMA.COLUMNS c
            LEFT JOIN (
                SELECT ku.TABLE_SCHEMA, ku.TABLE_NAME, ku.COLUMN_NAME
                FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS tc
                JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE ku
                    ON tc.CONSTRAINT_NAME = ku.CONSTRAINT_NAME
                   AND tc.TABLE_SCHEMA = ku.TABLE_SCHEMA
                WHERE tc.CONSTRAINT_TYPE = 'PRIMARY KEY'
            ) pk ON pk.TABLE_SCHEMA = c.TABLE_SCHEMA
                AND pk.TABLE_NAME = c.TABLE_NAME
                AND pk.COLUMN_NAME = c.COLUMN_NAME
            WHERE c.TABLE_NAME = @table
              AND (@schema IS NULL OR c.TABLE_SCHEMA = @schema)
            ORDER BY c.ORDINAL_POSITION;";

        var columns = new List<DatabaseColumn>();
        await using var conn = new SqlConnection(connectionString ?? _defaultConnectionString);
        await conn.OpenAsync(ct);
        await using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@table", name);
        cmd.Parameters.AddWithValue("@schema", (object?)schema ?? DBNull.Value);
        await using var reader = await cmd.ExecuteReaderAsync(ct);
        while (await reader.ReadAsync(ct))
        {
            columns.Add(new DatabaseColumn(
                TableName: name,
                Name: reader.GetString(0),
                DataType: reader.GetString(1),
                IsNullable: reader.GetString(2).Equals("YES", StringComparison.OrdinalIgnoreCase),
                MaxLength: reader.IsDBNull(3) ? null : reader.GetInt32(3),
                IsPrimaryKey: reader.GetInt32(4) == 1));
        }
        return columns;
    }

    public async Task<bool> ColumnExistsAsync(
        string tableName, string columnName, string? connectionString = null, CancellationToken ct = default)
    {
        var columns = await GetColumnsAsync(tableName, connectionString, ct);
        return columns.Any(c => c.Name.Equals(columnName, StringComparison.OrdinalIgnoreCase));
    }

    private static (string? schema, string name) SplitTableName(string tableName)
    {
        var parts = tableName.Split('.', 2);
        return parts.Length == 2 ? (parts[0], parts[1]) : (null, tableName);
    }
}
