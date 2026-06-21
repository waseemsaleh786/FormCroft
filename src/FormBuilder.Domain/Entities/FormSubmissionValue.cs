using FormBuilder.Domain.Common;

namespace FormBuilder.Domain.Entities;

/// <summary>The value captured for one field within a submission.</summary>
public class FormSubmissionValue : BaseEntity
{
    private FormSubmissionValue() { }

    public FormSubmissionValue(Guid formFieldId, string? value)
    {
        FormFieldId = formFieldId;
        Value = value;
    }

    public Guid SubmissionId { get; private set; }
    public Guid FormFieldId { get; private set; }
    public string? Value { get; private set; }
}
