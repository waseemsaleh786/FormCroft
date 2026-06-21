using FormBuilder.Domain.Enums;

namespace FormBuilder.Web.Models.Designer;

/// <summary>Mutable client-side editing model for the Form Designer.</summary>
public class DesignerForm
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Slug { get; set; } = string.Empty;
    public FormStatus Status { get; set; }
    public List<DesignerPage> Pages { get; set; } = new();
}

public class DesignerPage
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = "Page";
    public List<DesignerRow> Rows { get; set; } = new();
    public int FieldCount => Rows.SelectMany(r => r.Columns).SelectMany(c => c.Fields).Count();
}

public class DesignerRow
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public List<DesignerColumn> Columns { get; set; } = new();
}

public class DesignerColumn
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public int Width { get; set; } = 12;
    public List<DesignerField> Fields { get; set; } = new();

    /// <summary>Nested rows placed inside this column (nested layouts).</summary>
    public List<DesignerRow> SubRows { get; set; } = new();

    public bool IsEmpty => Fields.Count == 0 && SubRows.Count == 0;
}

public class DesignerField
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public FieldType Type { get; set; }
    public bool Required { get; set; }
    public string? Placeholder { get; set; }
    public string? HelpText { get; set; }
    public string? DefaultValue { get; set; }

    // Validation
    public int? MinLength { get; set; }
    public int? MaxLength { get; set; }
    public string? Pattern { get; set; }
    public string? ErrorMessage { get; set; }

    // Appearance
    public bool HideLabel { get; set; }
    public string? CssClass { get; set; }
    public string? Tooltip { get; set; }

    // Conditional logic
    public bool ConditionalEnabled { get; set; }
    public string? DependsOnField { get; set; }
    public ConditionalOperator ConditionalOperator { get; set; }
    public string? ConditionalValue { get; set; }

    // Options for choice fields (newline-friendly editor binds to OptionsText)
    public List<string> Options { get; set; } = new();

    public bool IsChoice => Type is FieldType.Dropdown or FieldType.Radio or FieldType.Checkbox or FieldType.MultiSelect;
    public bool IsStatic => Type is FieldType.Heading or FieldType.Paragraph or FieldType.Divider or FieldType.Button;
}
