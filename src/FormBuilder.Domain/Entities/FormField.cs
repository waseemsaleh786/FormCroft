using FormBuilder.Domain.Common;
using FormBuilder.Domain.Enums;
using FormBuilder.Domain.Exceptions;

namespace FormBuilder.Domain.Entities;

/// <summary>
/// A single field placed in a column. Validation and presentation settings are
/// stored as JSON (<see cref="ValidationJson"/>, <see cref="SettingsJson"/>) so the
/// schema can evolve without migrations; the Application layer maps them to the
/// <c>FieldValidation</c> / <c>FieldSettings</c> value objects.
/// </summary>
public class FormField : BaseEntity
{
    private FormField() { }

    public FormField(string name, string label, FieldType type, int order, Guid? id = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Field name is required.");
        if (id.HasValue && id.Value != Guid.Empty) Id = id.Value;
        Name = name.Trim();
        Label = label?.Trim() ?? name;
        Type = type;
        Order = order;
    }

    public Guid ColumnId { get; private set; }

    public string Name { get; private set; } = string.Empty;
    public string Label { get; private set; } = string.Empty;
    public FieldType Type { get; private set; }
    public bool Required { get; private set; }

    public string? Placeholder { get; private set; }
    public string? HelpText { get; private set; }
    public string? DefaultValue { get; private set; }

    public int Order { get; private set; }

    public string? ValidationJson { get; private set; }
    public string? SettingsJson { get; private set; }

    public void Configure(
        string label,
        bool required,
        string? placeholder,
        string? helpText,
        string? defaultValue,
        string? validationJson,
        string? settingsJson)
    {
        Label = label?.Trim() ?? Label;
        Required = required;
        Placeholder = placeholder;
        HelpText = helpText;
        DefaultValue = defaultValue;
        ValidationJson = validationJson;
        SettingsJson = settingsJson;
        Touch();
    }

    public void SetOrder(int order) => Order = order;
}
