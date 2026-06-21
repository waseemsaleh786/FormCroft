using AutoMapper;
using FormBuilder.Application.DTOs;
using FormBuilder.Domain.Interfaces;
using MediatR;

namespace FormBuilder.Application.Features.Forms.Queries;

/// <summary>Lists forms for the library, optionally filtered by a name search term.</summary>
public sealed record GetFormsQuery(string? Search = null) : IRequest<IReadOnlyList<FormSummaryDto>>;

public sealed class GetFormsQueryHandler
    : IRequestHandler<GetFormsQuery, IReadOnlyList<FormSummaryDto>>
{
    private readonly IFormRepository _forms;
    private readonly IFormSubmissionRepository _submissions;
    private readonly IMapper _mapper;

    public GetFormsQueryHandler(
        IFormRepository forms,
        IFormSubmissionRepository submissions,
        IMapper mapper)
    {
        _forms = forms;
        _submissions = submissions;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<FormSummaryDto>> Handle(GetFormsQuery request, CancellationToken ct)
    {
        var forms = await _forms.ListAsync(request.Search, ct);
        var result = new List<FormSummaryDto>(forms.Count);

        foreach (var form in forms)
        {
            var dto = _mapper.Map<FormSummaryDto>(form);
            var count = await _submissions.CountByFormAsync(form.Id, ct);
            result.Add(dto with { SubmissionCount = count });
        }

        return result;
    }
}
