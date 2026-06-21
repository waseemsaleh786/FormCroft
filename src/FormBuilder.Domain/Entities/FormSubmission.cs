using FormBuilder.Domain.Common;
using FormBuilder.Domain.Events;

namespace FormBuilder.Domain.Entities;

/// <summary>A single submission of a form, holding one value per answered field.</summary>
public class FormSubmission : BaseEntity
{
    private readonly List<FormSubmissionValue> _values = new();

    private FormSubmission() { }

    private FormSubmission(Guid formId)
    {
        FormId = formId;
        SubmittedAt = DateTime.UtcNow;
    }

    public Guid FormId { get; private set; }
    public DateTime SubmittedAt { get; private set; }

    public IReadOnlyCollection<FormSubmissionValue> Values => _values.AsReadOnly();

    public static FormSubmission Create(Guid formId, IEnumerable<(Guid fieldId, string? value)> values)
    {
        var submission = new FormSubmission(formId);
        foreach (var (fieldId, value) in values)
            submission._values.Add(new FormSubmissionValue(fieldId, value));
        submission.Raise(new FormSubmittedEvent(formId, submission.Id));
        return submission;
    }
}
