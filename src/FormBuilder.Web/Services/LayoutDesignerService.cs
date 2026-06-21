using FormBuilder.Domain.Enums;
using FormBuilder.Web.Models.Designer;

namespace FormBuilder.Web.Services;

/// <summary>
/// Stateless factory for designer building blocks: layout templates, rows built
/// from a template, and new fields. Mirrors the prototype's PLACEHOLDER_TYPES /
/// field creation logic.
/// </summary>
public class LayoutDesignerService
{
    public IReadOnlyList<LayoutTemplate> Templates => FieldCatalog.Layouts;
    public IReadOnlyList<FieldTypeInfo> FieldTypes => FieldCatalog.Fields;
    public IEnumerable<string> FieldGroups => FieldCatalog.Groups;

    /// <summary>Builds a fresh row (with empty columns) from a layout template.</summary>
    public DesignerRow BuildRow(LayoutTemplate template) => new()
    {
        Columns = template.Columns.Select(w => new DesignerColumn { Width = w }).ToList()
    };

    /// <summary>Creates a new field of the given type with a unique generated name.</summary>
    public DesignerField CreateField(FieldType type, int existingFieldCount)
    {
        var info = FieldCatalog.Info(type);
        return new DesignerField
        {
            Type = type,
            Label = info.Label,
            Name = $"{type.ToString().ToLowerInvariant()}_{existingFieldCount + 1}"
        };
    }
}
