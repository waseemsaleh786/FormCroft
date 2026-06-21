using System.Text.Json;
using FormBuilder.Domain.ValueObjects;

namespace FormBuilder.Application.Common;

/// <summary>
/// (De)serializes the typed field value objects to/from the JSON columns
/// (<c>ValidationJson</c>, <c>SettingsJson</c>) stored on <c>FormField</c>.
/// </summary>
public static class FieldConfigSerializer
{
    private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web);

    public static string? Serialize(FieldValidation? validation)
        => validation is null ? null : JsonSerializer.Serialize(validation, Options);

    public static string? Serialize(FieldSettings? settings)
        => settings is null ? null : JsonSerializer.Serialize(settings, Options);

    public static FieldValidation DeserializeValidation(string? json)
    {
        if (string.IsNullOrWhiteSpace(json)) return FieldValidation.Empty;
        try { return JsonSerializer.Deserialize<FieldValidation>(json, Options) ?? FieldValidation.Empty; }
        catch { return FieldValidation.Empty; }
    }

    public static FieldSettings DeserializeSettings(string? json)
    {
        if (string.IsNullOrWhiteSpace(json)) return new FieldSettings();
        try { return JsonSerializer.Deserialize<FieldSettings>(json, Options) ?? new FieldSettings(); }
        catch { return new FieldSettings(); }
    }
}
