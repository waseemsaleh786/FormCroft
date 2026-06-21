using FormBuilder.Application.DTOs;
using FormBuilder.Domain.ValueObjects;

namespace FormBuilder.Web.Models.Designer;

/// <summary>Converts between the Application <see cref="FormDesignerDto"/> and the mutable editing model.</summary>
public static class DesignerMapper
{
    public static DesignerForm ToModel(FormDesignerDto dto) => new()
    {
        Id = dto.Id,
        Name = dto.Name,
        Description = dto.Description,
        Slug = dto.Slug,
        Status = dto.Status,
        Pages = dto.Pages.OrderBy(p => p.Order).Select(ToModel).ToList()
    };

    private static DesignerPage ToModel(PageDto p) => new()
    {
        Id = p.Id,
        Name = p.Name,
        Rows = p.Rows.OrderBy(r => r.Order).Select(ToModel).ToList()
    };

    private static DesignerRow ToModel(RowDto r) => new()
    {
        Id = r.Id,
        Columns = r.Columns.OrderBy(c => c.Order).Select(ToModel).ToList()
    };

    private static DesignerColumn ToModel(ColumnDto c) => new()
    {
        Id = c.Id,
        Width = c.Width,
        Fields = c.Fields.OrderBy(f => f.Order).Select(ToModel).ToList(),
        SubRows = c.SubRows.OrderBy(r => r.Order).Select(ToModel).ToList()
    };

    private static DesignerField ToModel(FieldDto f) => new()
    {
        Id = f.Id,
        Name = f.Name,
        Label = f.Label,
        Type = f.Type,
        Required = f.Required,
        Placeholder = f.Placeholder,
        HelpText = f.HelpText,
        DefaultValue = f.DefaultValue,
        MinLength = f.Validation.MinLength,
        MaxLength = f.Validation.MaxLength,
        Pattern = f.Validation.Pattern,
        ErrorMessage = f.Validation.ErrorMessage,
        HideLabel = f.Settings.HideLabel,
        CssClass = f.Settings.CssClass,
        Tooltip = f.Settings.Tooltip,
        ConditionalEnabled = f.Settings.Conditional?.Enabled ?? false,
        DependsOnField = f.Settings.Conditional?.DependsOnField,
        ConditionalOperator = f.Settings.Conditional?.Operator ?? default,
        ConditionalValue = f.Settings.Conditional?.Value,
        Options = f.Settings.Options.Select(o => o.Label).ToList()
    };

    public static FormDesignerDto ToDto(DesignerForm m) => new()
    {
        Id = m.Id,
        Name = m.Name,
        Description = m.Description,
        Slug = m.Slug,
        Status = m.Status,
        Pages = m.Pages.Select((p, pi) => new PageDto
        {
            Id = p.Id,
            Name = p.Name,
            Order = pi,
            Rows = p.Rows.Select((r, ri) => new RowDto
            {
                Id = r.Id,
                Order = ri,
                Columns = r.Columns.Select((c, ci) => ToDto(c, ci)).ToList()
            }).ToList()
        }).ToList()
    };

    private static RowDto ToDto(DesignerRow r, int order) => new()
    {
        Id = r.Id,
        Order = order,
        Columns = r.Columns.Select((c, ci) => ToDto(c, ci)).ToList()
    };

    private static ColumnDto ToDto(DesignerColumn c, int order) => new()
    {
        Id = c.Id,
        Width = c.Width,
        Order = order,
        Fields = c.Fields.Select((f, fi) => ToDto(f, fi)).ToList(),
        SubRows = c.SubRows.Select((sr, sri) => ToDto(sr, sri)).ToList()
    };

    private static FieldDto ToDto(DesignerField f, int order) => new()
    {
        Id = f.Id,
        Name = f.Name,
        Label = f.Label,
        Type = f.Type,
        Required = f.Required,
        Placeholder = f.Placeholder,
        HelpText = f.HelpText,
        DefaultValue = f.DefaultValue,
        Order = order,
        Validation = new FieldValidation
        {
            MinLength = f.MinLength,
            MaxLength = f.MaxLength,
            Pattern = f.Pattern,
            ErrorMessage = f.ErrorMessage
        },
        Settings = new FieldSettings
        {
            HideLabel = f.HideLabel,
            CssClass = f.CssClass,
            Tooltip = f.Tooltip,
            Conditional = f.ConditionalEnabled
                ? new ConditionalLogic
                {
                    Enabled = true,
                    DependsOnField = f.DependsOnField,
                    Operator = f.ConditionalOperator,
                    Value = f.ConditionalValue
                }
                : null,
            Options = f.Options
                .Where(o => !string.IsNullOrWhiteSpace(o))
                .Select(o => new FieldOption(o, o)).ToList()
        }
    };
}
