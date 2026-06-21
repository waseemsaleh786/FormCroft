using FormBuilder.Domain.Enums;

namespace FormBuilder.Domain.ValueObjects;

/// <summary>
/// Conditional-display rule for a field. When <see cref="Enabled"/> is true the
/// field is only shown when the field named <see cref="DependsOnField"/> satisfies
/// <see cref="Operator"/> against <see cref="Value"/>.
/// </summary>
public sealed record ConditionalLogic
{
    public bool Enabled { get; init; }
    public string? DependsOnField { get; init; }
    public ConditionalOperator Operator { get; init; }
    public string? Value { get; init; }
}
