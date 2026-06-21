using FormBuilder.Application.Common;
using FormBuilder.Application.DTOs;
using FormBuilder.Domain.Entities;

namespace FormBuilder.Application.Features.Designer;

/// <summary>Recursive entity → DTO projection for the designer (supports nested sub-rows).</summary>
public static class DesignerProjection
{
    public static FormDesignerDto MapForm(Form form) => new()
    {
        Id = form.Id,
        Name = form.Name,
        Description = form.Description,
        Slug = form.Slug,
        Status = form.Status,
        Pages = form.Pages.OrderBy(p => p.Order).Select(MapPage).ToList()
    };

    public static PageDto MapPage(FormPage page) => new()
    {
        Id = page.Id,
        Name = page.Name,
        Order = page.Order,
        Rows = page.Rows.OrderBy(r => r.Order).Select(MapRow).ToList()
    };

    public static RowDto MapRow(FormRow row) => new()
    {
        Id = row.Id,
        Order = row.Order,
        Columns = row.Columns.OrderBy(c => c.Order).Select(MapColumn).ToList()
    };

    public static ColumnDto MapColumn(FormColumn column) => new()
    {
        Id = column.Id,
        Width = column.Width,
        Order = column.Order,
        Fields = column.Fields.OrderBy(f => f.Order).Select(MapField).ToList(),
        SubRows = column.SubRows.OrderBy(r => r.Order).Select(MapRow).ToList()
    };

    public static FieldDto MapField(FormField field) => new()
    {
        Id = field.Id,
        Name = field.Name,
        Label = field.Label,
        Type = field.Type,
        Required = field.Required,
        Placeholder = field.Placeholder,
        HelpText = field.HelpText,
        DefaultValue = field.DefaultValue,
        Order = field.Order,
        Validation = FieldConfigSerializer.DeserializeValidation(field.ValidationJson),
        Settings = FieldConfigSerializer.DeserializeSettings(field.SettingsJson)
    };
}
