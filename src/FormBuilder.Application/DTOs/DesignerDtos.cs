using FormBuilder.Domain.Enums;
using FormBuilder.Domain.ValueObjects;

namespace FormBuilder.Application.DTOs;

/// <summary>Full designer payload: a form plus its page → row → column → field tree.</summary>
public sealed record FormDesignerDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string Slug { get; init; } = string.Empty;
    public FormStatus Status { get; init; }
    public List<PageDto> Pages { get; init; } = new();
}

public sealed record PageDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public int Order { get; init; }
    public List<RowDto> Rows { get; init; } = new();
}

public sealed record RowDto
{
    public Guid Id { get; init; }
    public int Order { get; init; }
    public List<ColumnDto> Columns { get; init; } = new();
}

public sealed record ColumnDto
{
    public Guid Id { get; init; }
    public int Width { get; init; }
    public int Order { get; init; }
    public List<FieldDto> Fields { get; init; } = new();

    /// <summary>Nested rows placed inside this column (nested layouts).</summary>
    public List<RowDto> SubRows { get; init; } = new();
}

public sealed record FieldDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Label { get; init; } = string.Empty;
    public FieldType Type { get; init; }
    public bool Required { get; init; }
    public string? Placeholder { get; init; }
    public string? HelpText { get; init; }
    public string? DefaultValue { get; init; }
    public int Order { get; init; }
    public FieldValidation Validation { get; init; } = FieldValidation.Empty;
    public FieldSettings Settings { get; init; } = new();
}
