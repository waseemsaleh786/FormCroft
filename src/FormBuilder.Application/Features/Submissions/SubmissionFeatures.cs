using FormBuilder.Application.Common.Exceptions;
using FormBuilder.Application.DTOs;
using FormBuilder.Domain.Entities;
using FormBuilder.Domain.Exceptions;
using FormBuilder.Domain.Interfaces;
using MediatR;

namespace FormBuilder.Application.Features.Submissions;

// ---- Command: submit a published form (runtime) ----
public sealed record SubmitFormCommand(string Slug, IReadOnlyDictionary<Guid, string?> Values) : IRequest<Guid>;

public sealed class SubmitFormCommandHandler : IRequestHandler<SubmitFormCommand, Guid>
{
    private readonly IFormRepository _forms;
    private readonly IFormSubmissionRepository _submissions;
    private readonly IUnitOfWork _uow;

    public SubmitFormCommandHandler(
        IFormRepository forms, IFormSubmissionRepository submissions, IUnitOfWork uow)
    {
        _forms = forms;
        _submissions = submissions;
        _uow = uow;
    }

    public async Task<Guid> Handle(SubmitFormCommand request, CancellationToken ct)
    {
        var form = await _forms.GetPublishedBySlugAsync(request.Slug, ct)
                   ?? throw new NotFoundException($"Published form '{request.Slug}' was not found.");

        var fields = form.Pages.SelectMany(p => p.Rows).SelectMany(EnumerateRow).ToList();

        // Enforce required fields server-side.
        foreach (var field in fields.Where(f => f.Required))
        {
            request.Values.TryGetValue(field.Id, out var value);
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException($"'{field.Label}' is required.");
        }

        var values = fields
            .Where(f => request.Values.ContainsKey(f.Id))
            .Select(f => (f.Id, request.Values[f.Id]));

        var submission = FormSubmission.Create(form.Id, values);
        await _submissions.AddAsync(submission, ct);
        await _uow.SaveChangesAsync(ct);
        return submission.Id;
    }

    private static IEnumerable<FormField> EnumerateRow(FormRow row) =>
        row.Columns.SelectMany(c => c.Fields.Concat(c.SubRows.SelectMany(EnumerateRow)));
}

// ---- Query: a single submission ----
public sealed record GetSubmissionQuery(Guid Id) : IRequest<SubmissionDto>;

public sealed class GetSubmissionQueryHandler : IRequestHandler<GetSubmissionQuery, SubmissionDto>
{
    private readonly IFormSubmissionRepository _submissions;
    public GetSubmissionQueryHandler(IFormSubmissionRepository submissions) => _submissions = submissions;

    public async Task<SubmissionDto> Handle(GetSubmissionQuery request, CancellationToken ct)
    {
        var s = await _submissions.GetAsync(request.Id, ct)
                ?? throw new NotFoundException(nameof(FormSubmission), request.Id);
        return Map(s);
    }

    internal static SubmissionDto Map(FormSubmission s) => new()
    {
        Id = s.Id,
        FormId = s.FormId,
        SubmittedAt = s.SubmittedAt,
        Values = s.Values.Select(v => new SubmissionValueDto
        {
            FormFieldId = v.FormFieldId,
            Value = v.Value
        }).ToList()
    };
}

// ---- Query: all submissions for a form ----
public sealed record GetSubmissionsQuery(Guid FormId) : IRequest<IReadOnlyList<SubmissionDto>>;

public sealed class GetSubmissionsQueryHandler
    : IRequestHandler<GetSubmissionsQuery, IReadOnlyList<SubmissionDto>>
{
    private readonly IFormSubmissionRepository _submissions;
    public GetSubmissionsQueryHandler(IFormSubmissionRepository submissions) => _submissions = submissions;

    public async Task<IReadOnlyList<SubmissionDto>> Handle(GetSubmissionsQuery request, CancellationToken ct)
    {
        var list = await _submissions.GetByFormAsync(request.FormId, ct);
        return list.Select(GetSubmissionQueryHandler.Map).ToList();
    }
}
