using FormBuilder.Application.Common.Exceptions;
using FormBuilder.Application.DTOs;
using FormBuilder.Domain.Entities;
using FormBuilder.Domain.Interfaces;
using MediatR;

namespace FormBuilder.Application.Features.Designer.Queries;

/// <summary>Loads the full designer payload (form + page/row/column/field tree, incl. nesting).</summary>
public sealed record GetFormDesignerQuery(Guid FormId) : IRequest<FormDesignerDto>;

public sealed class GetFormDesignerQueryHandler : IRequestHandler<GetFormDesignerQuery, FormDesignerDto>
{
    private readonly IFormRepository _forms;

    public GetFormDesignerQueryHandler(IFormRepository forms) => _forms = forms;

    public async Task<FormDesignerDto> Handle(GetFormDesignerQuery request, CancellationToken ct)
    {
        var form = await _forms.GetWithLayoutAsync(request.FormId, ct)
                   ?? throw new NotFoundException(nameof(Form), request.FormId);

        return DesignerProjection.MapForm(form);
    }
}
