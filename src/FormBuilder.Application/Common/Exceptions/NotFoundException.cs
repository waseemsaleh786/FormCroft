namespace FormBuilder.Application.Common.Exceptions;

/// <summary>Thrown when a requested aggregate cannot be found.</summary>
public class NotFoundException : Exception
{
    public NotFoundException(string entity, object key)
        : base($"{entity} with key '{key}' was not found.") { }

    public NotFoundException(string message) : base(message) { }
}
