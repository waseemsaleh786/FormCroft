namespace FormBuilder.Domain.ValueObjects;

/// <summary>
/// Typed representation of a field's validation rules. Serialized to
/// <see cref="Entities.FormField.ValidationJson"/> for storage.
/// </summary>
public sealed record FieldValidation
{
    public int? MinLength { get; init; }
    public int? MaxLength { get; init; }
    public string? Pattern { get; init; }
    public string? ErrorMessage { get; init; }

    public static FieldValidation Empty => new();
}
