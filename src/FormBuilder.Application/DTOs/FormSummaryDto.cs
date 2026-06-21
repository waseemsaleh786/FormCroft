using FormBuilder.Domain.Enums;

namespace FormBuilder.Application.DTOs;

/// <summary>Card/grid projection for the Forms Library list.</summary>
public sealed record FormSummaryDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string Slug { get; init; } = string.Empty;
    public FormStatus Status { get; init; }
    public int PageCount { get; init; }
    public int FieldCount { get; init; }
    public int SubmissionCount { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}
