namespace FormBuilder.Domain.Enums;

/// <summary>
/// Supported form field types. Values are ordered by the toolbox grouping
/// used in the designer (Basic, Choice, Advanced, Static).
/// </summary>
public enum FieldType
{
    // Basic inputs
    Text = 0,
    Textarea = 1,
    Email = 2,
    Phone = 3,
    Password = 4,
    Number = 5,

    // Choice inputs
    Dropdown = 10,
    Radio = 11,
    Checkbox = 12,
    MultiSelect = 13,

    // Advanced inputs
    Date = 20,
    DateTime = 21,
    FileUpload = 22,

    // Static / layout content
    Heading = 30,
    Paragraph = 31,
    Divider = 32,
    Button = 33
}
