namespace FormBuilder.Domain.Entities;

/// <summary>Read model describing a database table discovered via INFORMATION_SCHEMA.</summary>
public sealed record DatabaseTable(string Schema, string Name)
{
    public string FullName => Schema == "dbo" ? Name : $"{Schema}.{Name}";
}

/// <summary>Read model describing a column within a <see cref="DatabaseTable"/>.</summary>
public sealed record DatabaseColumn(
    string TableName,
    string Name,
    string DataType,
    bool IsNullable,
    int? MaxLength,
    bool IsPrimaryKey);
