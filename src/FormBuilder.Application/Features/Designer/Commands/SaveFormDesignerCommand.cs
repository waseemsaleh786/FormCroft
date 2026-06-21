using FluentValidation;
using FormBuilder.Application.Common;
using FormBuilder.Application.Common.Exceptions;
using FormBuilder.Application.DTOs;
using FormBuilder.Domain.Entities;
using FormBuilder.Domain.Interfaces;
using MediatR;

namespace FormBuilder.Application.Features.Designer.Commands;

/// <summary>
/// Persists the entire designer layout for a form. Rebuilds the page tree from
/// the supplied DTO, preserving entity IDs so existing field mappings stay valid.
/// </summary>
public sealed record SaveFormDesignerCommand(FormDesignerDto Design) : IRequest;

public sealed class SaveFormDesignerCommandValidator : AbstractValidator<SaveFormDesignerCommand>
{
    public SaveFormDesignerCommandValidator()
    {
        RuleFor(x => x.Design).NotNull();
        RuleFor(x => x.Design.Id).NotEmpty();
        RuleFor(x => x.Design.Name).NotEmpty().MaximumLength(200);
        RuleForEach(x => x.Design.Pages).ChildRules(page =>
        {
            page.RuleFor(p => p.Name).NotEmpty().WithMessage("Each page needs a name.");
            page.RuleForEach(p => p.Rows).ChildRules(row =>
                row.RuleFor(r => r.Columns).Must(cols => cols.Sum(c => c.Width) <= 12)
                    .WithMessage("Column widths in a row cannot exceed 12."));
        });
    }
}

public sealed class SaveFormDesignerCommandHandler : IRequestHandler<SaveFormDesignerCommand>
{
    private readonly IFormRepository _forms;
    private readonly IUnitOfWork _uow;

    public SaveFormDesignerCommandHandler(IFormRepository forms, IUnitOfWork uow)
    {
        _forms = forms;
        _uow = uow;
    }

    public async Task Handle(SaveFormDesignerCommand request, CancellationToken ct)
    {
        var design = request.Design;
        var form = await _forms.GetWithLayoutAsync(design.Id, ct)
                   ?? throw new NotFoundException(nameof(Form), design.Id);

        if (!string.Equals(form.Name, design.Name, StringComparison.Ordinal))
            form.Rename(design.Name);
        form.UpdateDescription(design.Description);

        var pages = design.Pages
            .OrderBy(p => p.Order)
            .Select((p, i) => BuildPage(p, i))
            .ToList();

        form.ReplacePages(pages);
        await _uow.SaveChangesAsync(ct);
    }

    private static FormPage BuildPage(PageDto dto, int order)
    {
        var page = new FormPage(dto.Name, order, dto.Id);
        var rowOrder = 0;
        foreach (var rowDto in dto.Rows.OrderBy(r => r.Order))
            page.AddRow(BuildRow(rowDto, rowOrder++));
        return page;
    }

    private static FormRow BuildRow(RowDto dto, int order)
    {
        var row = new FormRow(order, dto.Id);
        var colOrder = 0;
        foreach (var colDto in dto.Columns.OrderBy(c => c.Order))
            row.AddColumn(BuildColumn(colDto, colOrder++));
        return row;
    }

    private static FormColumn BuildColumn(ColumnDto dto, int order)
    {
        var column = new FormColumn(dto.Width, order, dto.Id);
        var fieldOrder = 0;
        foreach (var fieldDto in dto.Fields.OrderBy(f => f.Order))
            column.AddField(BuildField(fieldDto, fieldOrder++));
        var subRowOrder = 0;
        foreach (var subRowDto in dto.SubRows.OrderBy(r => r.Order))
            column.AddSubRow(BuildRow(subRowDto, subRowOrder++));
        return column;
    }

    private static FormField BuildField(FieldDto dto, int order)
    {
        var name = string.IsNullOrWhiteSpace(dto.Name) ? $"field_{Guid.NewGuid():n}"[..16] : dto.Name;
        var field = new FormField(name, dto.Label, dto.Type, order, dto.Id);
        field.Configure(
            dto.Label,
            dto.Required,
            dto.Placeholder,
            dto.HelpText,
            dto.DefaultValue,
            FieldConfigSerializer.Serialize(dto.Validation),
            FieldConfigSerializer.Serialize(dto.Settings));
        return field;
    }
}
