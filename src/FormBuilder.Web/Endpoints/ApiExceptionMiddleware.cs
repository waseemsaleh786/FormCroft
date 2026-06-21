using FormBuilder.Application.Common.Exceptions;
using FormBuilder.Domain.Exceptions;

namespace FormBuilder.Web.Endpoints;

/// <summary>
/// Translates known exceptions into JSON problem responses for the /api surface.
/// Domain/validation rule violations → 400, missing aggregates → 404.
/// (Blazor circuits handle their own errors, so this only wraps /api requests.)
/// </summary>
public class ApiExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ApiExceptionMiddleware> _logger;

    public ApiExceptionMiddleware(RequestDelegate next, ILogger<ApiExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        if (!context.Request.Path.StartsWithSegments("/api"))
        {
            await _next(context);
            return;
        }

        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            await Write(context, StatusCodes.Status400BadRequest, "Validation failed", ex.Errors);
        }
        catch (DomainException ex)
        {
            await Write(context, StatusCodes.Status400BadRequest, ex.Message);
        }
        catch (NotFoundException ex)
        {
            await Write(context, StatusCodes.Status404NotFound, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled API error");
            await Write(context, StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
        }
    }

    private static async Task Write(HttpContext context, int status, string message, object? errors = null)
    {
        context.Response.StatusCode = status;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(new { status, message, errors });
    }
}
