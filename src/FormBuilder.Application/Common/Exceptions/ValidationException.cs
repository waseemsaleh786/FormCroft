using FluentValidation.Results;

namespace FormBuilder.Application.Common.Exceptions;

/// <summary>Aggregates FluentValidation failures from the validation pipeline.</summary>
public class ValidationException : Exception
{
    public ValidationException() : base("One or more validation failures occurred.")
        => Errors = new Dictionary<string, string[]>();

    public ValidationException(IEnumerable<ValidationFailure> failures) : this()
        => Errors = failures
            .GroupBy(f => f.PropertyName, f => f.ErrorMessage)
            .ToDictionary(g => g.Key, g => g.ToArray());

    public IDictionary<string, string[]> Errors { get; }
}
