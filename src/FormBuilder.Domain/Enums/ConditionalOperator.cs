namespace FormBuilder.Domain.Enums;

/// <summary>Operators available for a field's conditional-display logic.</summary>
public enum ConditionalOperator
{
    Equals = 0,
    NotEquals = 1,
    Contains = 2,
    GreaterThan = 3,
    LessThan = 4
}
