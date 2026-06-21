using FormBuilder.Domain.Enums;

namespace FormBuilder.Web.Models.Designer;

public sealed record FieldTypeInfo(FieldType Type, string Label, string Icon, string Group);

public sealed record LayoutTemplate(string Id, string Label, string Description, int[] Columns);

/// <summary>Toolbox metadata: the supported field types grouped for the right panel.</summary>
public static class FieldCatalog
{
    public const string Basic = "Basic";
    public const string Choice = "Choice";
    public const string Advanced = "Advanced";
    public const string Static = "Static";

    public static readonly IReadOnlyList<FieldTypeInfo> Fields = new List<FieldTypeInfo>
    {
        new(FieldType.Text,       "Text",        "Aa",  Basic),
        new(FieldType.Textarea,   "Textarea",    "¶",   Basic),
        new(FieldType.Email,      "Email",       "@",   Basic),
        new(FieldType.Phone,      "Phone",       "☎",   Basic),
        new(FieldType.Password,   "Password",    "🔒",  Basic),
        new(FieldType.Number,     "Number",      "#",   Basic),

        new(FieldType.Dropdown,   "Dropdown",    "▾",   Choice),
        new(FieldType.Radio,      "Radio",       "◉",   Choice),
        new(FieldType.Checkbox,   "Checkbox",    "☑",   Choice),
        new(FieldType.MultiSelect,"MultiSelect", "☰",   Choice),

        new(FieldType.Date,       "Date",        "📅",  Advanced),
        new(FieldType.DateTime,   "DateTime",    "🕐",  Advanced),
        new(FieldType.FileUpload, "File Upload", "📎",  Advanced),

        new(FieldType.Heading,    "Heading",     "H",   Static),
        new(FieldType.Paragraph,  "Paragraph",   "¶",   Static),
        new(FieldType.Divider,    "Divider",     "―",   Static),
        new(FieldType.Button,     "Button",      "🔘",  Static),
    };

    public static IEnumerable<string> Groups => new[] { Basic, Choice, Advanced, Static };

    public static FieldTypeInfo Info(FieldType type) => Fields.First(f => f.Type == type);

    /// <summary>Layout templates draggable onto the canvas (12-grid widths).</summary>
    public static readonly IReadOnlyList<LayoutTemplate> Layouts = new List<LayoutTemplate>
    {
        new("col-12",     "1 Column",   "Full width",            new[] { 12 }),
        new("col-6-6",    "2 Columns",  "Two equal (6 + 6)",     new[] { 6, 6 }),
        new("col-8-4",    "2 Columns",  "Wide + narrow (8 + 4)", new[] { 8, 4 }),
        new("col-4-8",    "2 Columns",  "Narrow + wide (4 + 8)", new[] { 4, 8 }),
        new("col-4-4-4",  "3 Columns",  "Three equal (4×3)",     new[] { 4, 4, 4 }),
        new("col-3-3-3-3","4 Columns",  "Four equal (3×4)",      new[] { 3, 3, 3, 3 }),
    };
}
