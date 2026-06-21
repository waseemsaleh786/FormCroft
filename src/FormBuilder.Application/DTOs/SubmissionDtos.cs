namespace FormBuilder.Application.DTOs;

public sealed record SubmissionDto
{
    public Guid Id { get; init; }
    public Guid FormId { get; init; }
    public DateTime SubmittedAt { get; init; }
    public List<SubmissionValueDto> Values { get; init; } = new();
}

public sealed record SubmissionValueDto
{
    public Guid FormFieldId { get; init; }
    public string? FieldLabel { get; init; }
    public string? Value { get; init; }
}
