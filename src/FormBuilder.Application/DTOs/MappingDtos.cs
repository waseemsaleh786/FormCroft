using FormBuilder.Domain.Enums;

namespace FormBuilder.Application.DTOs;

public sealed record FieldMappingDto
{
    public Guid Id { get; init; }
    public Guid FormId { get; init; }
    public Guid FormFieldId { get; init; }
    public string FormFieldLabel { get; init; } = string.Empty;
    public string TableName { get; init; } = string.Empty;
    public string ColumnName { get; init; } = string.Empty;
}

/// <summary>A mappable form field shown in the left panel of the mapping screen.</summary>
public sealed record MappableFieldDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Label { get; init; } = string.Empty;
    public FieldType Type { get; init; }
}

public sealed record DatabaseTableDto(string Schema, string Name, string FullName);

public sealed record DatabaseColumnDto(
    string Name,
    string DataType,
    bool IsNullable,
    int? MaxLength,
    bool IsPrimaryKey);
