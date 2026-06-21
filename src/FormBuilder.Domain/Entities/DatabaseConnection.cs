using FormBuilder.Domain.Common;
using FormBuilder.Domain.Exceptions;

namespace FormBuilder.Domain.Entities;

/// <summary>
/// A registered target database that form fields can be mapped into. The
/// connection string is read by <c>ITableMetadataService</c> to enumerate
/// tables and columns from INFORMATION_SCHEMA.
/// </summary>
public class DatabaseConnection : BaseEntity
{
    private DatabaseConnection() { }

    public DatabaseConnection(string name, string connectionString, string provider = "SqlServer")
    {
        if (string.IsNullOrWhiteSpace(name)) throw new DomainException("Connection name is required.");
        if (string.IsNullOrWhiteSpace(connectionString)) throw new DomainException("Connection string is required.");
        Name = name.Trim();
        ConnectionString = connectionString.Trim();
        Provider = provider;
    }

    public string Name { get; private set; } = string.Empty;
    public string ConnectionString { get; private set; } = string.Empty;
    public string Provider { get; private set; } = "SqlServer";
    public bool IsDefault { get; private set; }

    public void MarkDefault(bool isDefault) => IsDefault = isDefault;
}
