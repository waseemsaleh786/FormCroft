namespace FormBuilder.Domain.Enums;

/// <summary>Lifecycle state of a form. Transitions: Draft → Published → Archived.</summary>
public enum FormStatus
{
    Draft = 0,
    Published = 1,
    Archived = 2
}
