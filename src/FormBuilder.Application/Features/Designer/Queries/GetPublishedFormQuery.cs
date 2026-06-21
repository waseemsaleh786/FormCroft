using FormBuilder.Application.DTOs;
using FormBuilder.Domain.Interfaces;
using MediatR;

namespace FormBuilder.Application.Features.Designer.Queries;

/// <summary>Loads a published form by slug for the public runtime renderer. Null if not found/published.</summary>
public sealed record GetPublishedFormQuery(string Slug) : IRequest<FormDesignerDto?>;

public sealed class GetPublishedFormQueryHandler : IRequestHandler<GetPublishedFormQuery, FormDesignerDto?>
{
    private readonly IFormRepository _forms;

    public GetPublishedFormQueryHandler(IFormRepository forms) => _forms = forms;

    public async Task<FormDesignerDto?> Handle(GetPublishedFormQuery request, CancellationToken ct)
    {
        var form = await _forms.GetPublishedBySlugAsync(request.Slug, ct);
        return form is null ? null : DesignerProjection.MapForm(form);
    }
}
