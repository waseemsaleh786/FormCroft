using FormBuilder.Application.Common.Exceptions;
using FormBuilder.Application.DTOs;
using FormBuilder.Domain.Entities;
using FormBuilder.Domain.Interfaces;
using MediatR;

namespace FormBuilder.Application.Features.Mappings;

// ---- Query: mappable fields for a form (left panel) ----
public sealed record GetMappableFieldsQuery(Guid FormId) : IRequest<IReadOnlyList<MappableFieldDto>>;

public sealed class GetMappableFieldsQueryHandler
    : IRequestHandler<GetMappableFieldsQuery, IReadOnlyList<MappableFieldDto>>
{
    private readonly IFormRepository _forms;
    public GetMappableFieldsQueryHandler(IFormRepository forms) => _forms = forms;

    public async Task<IReadOnlyList<MappableFieldDto>> Handle(GetMappableFieldsQuery request, CancellationToken ct)
    {
        var form = await _forms.GetWithLayoutAsync(request.FormId, ct)
                   ?? throw new NotFoundException(nameof(Form), request.FormId);

        return EnumerateFields(form)
            .Select(f => new MappableFieldDto { Id = f.Id, Name = f.Name, Label = f.Label, Type = f.Type })
            .ToList();
    }

    internal static IEnumerable<FormField> EnumerateFields(Form form) =>
        form.Pages.SelectMany(p => p.Rows).SelectMany(EnumerateRow);

    private static IEnumerable<FormField> EnumerateRow(FormRow row) =>
        row.Columns.SelectMany(c => c.Fields.Concat(c.SubRows.SelectMany(EnumerateRow)));
}

// ---- Query: existing mappings for a form ----
public sealed record GetFieldMappingsQuery(Guid FormId) : IRequest<IReadOnlyList<FieldMappingDto>>;

public sealed class GetFieldMappingsQueryHandler
    : IRequestHandler<GetFieldMappingsQuery, IReadOnlyList<FieldMappingDto>>
{
    private readonly IFormRepository _forms;
    private readonly IFieldMappingRepository _mappings;

    public GetFieldMappingsQueryHandler(IFormRepository forms, IFieldMappingRepository mappings)
    {
        _forms = forms;
        _mappings = mappings;
    }

    public async Task<IReadOnlyList<FieldMappingDto>> Handle(GetFieldMappingsQuery request, CancellationToken ct)
    {
        var form = await _forms.GetWithLayoutAsync(request.FormId, ct)
                   ?? throw new NotFoundException(nameof(Form), request.FormId);

        var labels = GetMappableFieldsQueryHandler.EnumerateFields(form)
            .ToDictionary(f => f.Id, f => f.Label);

        var mappings = await _mappings.GetByFormAsync(request.FormId, ct);
        return mappings.Select(m => new FieldMappingDto
        {
            Id = m.Id,
            FormId = m.FormId,
            FormFieldId = m.FormFieldId,
            FormFieldLabel = labels.TryGetValue(m.FormFieldId, out var l) ? l : "(unknown field)",
            TableName = m.TableName,
            ColumnName = m.ColumnName
        }).ToList();
    }
}

// ---- Command: save (replace) all mappings for a form ----
public sealed record MappingItem(Guid FormFieldId, string TableName, string ColumnName);

public sealed record SaveFieldMappingsCommand(Guid FormId, IReadOnlyList<MappingItem> Mappings) : IRequest;

public sealed class SaveFieldMappingsCommandHandler : IRequestHandler<SaveFieldMappingsCommand>
{
    private readonly IFieldMappingRepository _mappings;
    private readonly ITableMetadataService _metadata;
    private readonly IUnitOfWork _uow;

    public SaveFieldMappingsCommandHandler(
        IFieldMappingRepository mappings, ITableMetadataService metadata, IUnitOfWork uow)
    {
        _mappings = mappings;
        _metadata = metadata;
        _uow = uow;
    }

    public async Task Handle(SaveFieldMappingsCommand request, CancellationToken ct)
    {
        // Rule: a form field may be mapped at most once.
        var duplicate = request.Mappings.GroupBy(m => m.FormFieldId).FirstOrDefault(g => g.Count() > 1);
        if (duplicate is not null)
            throw new Domain.Exceptions.DomainException("A form field can only be mapped to one column.");

        // Rule: each target column must actually exist.
        foreach (var item in request.Mappings)
        {
            if (!await _metadata.ColumnExistsAsync(item.TableName, item.ColumnName, ct: ct))
                throw new Domain.Exceptions.DomainException(
                    $"Column '{item.ColumnName}' does not exist on table '{item.TableName}'.");
        }

        var entities = request.Mappings
            .Select(m => new FieldMapping(request.FormId, m.FormFieldId, m.TableName, m.ColumnName))
            .ToList();

        await _mappings.ReplaceForFormAsync(request.FormId, entities, ct);
        await _uow.SaveChangesAsync(ct);
    }
}
