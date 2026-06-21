namespace FormBuilder.Domain.ValueObjects;

/// <summary>
/// Presentation/behaviour settings for a field plus the conditional logic.
/// Serialized to <see cref="Entities.FormField.SettingsJson"/>.
/// <see cref="Options"/> holds the choices for select/radio/checkbox fields.
/// </summary>
public sealed record FieldSettings
{
    public bool HideLabel { get; init; }
    public string? CssClass { get; init; }
    public string? Tooltip { get; init; }
    public ConditionalLogic? Conditional { get; init; }
    public IReadOnlyList<FieldOption> Options { get; init; } = Array.Empty<FieldOption>();
}

/// <summary>A single selectable option for choice fields.</summary>
public sealed record FieldOption(string Label, string Value);
