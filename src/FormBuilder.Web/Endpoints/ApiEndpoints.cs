using FormBuilder.Application.DTOs;
using FormBuilder.Application.Features.Designer.Commands;
using FormBuilder.Application.Features.Designer.Queries;
using FormBuilder.Application.Features.Forms.Commands;
using FormBuilder.Application.Features.Forms.Queries;
using FormBuilder.Application.Features.Mappings;
using FormBuilder.Application.Features.Metadata;
using FormBuilder.Application.Features.Submissions;
using MediatR;

namespace FormBuilder.Web.Endpoints;

/// <summary>
/// Minimal API surface over the MediatR use cases. The Blazor UI calls MediatR
/// directly; these endpoints expose the same operations to external clients.
/// </summary>
public static class ApiEndpoints
{
    public static IEndpointRouteBuilder MapFormBuilderApi(this IEndpointRouteBuilder app)
    {
        var forms = app.MapGroup("/api/forms").WithTags("Forms");

        forms.MapGet("/", async (string? search, ISender sender)
            => Results.Ok(await sender.Send(new GetFormsQuery(search))));

        forms.MapGet("/{id:guid}", async (Guid id, ISender sender)
            => Results.Ok(await sender.Send(new GetFormByIdQuery(id))));

        forms.MapPost("/", async (CreateFormRequest req, ISender sender) =>
        {
            var id = await sender.Send(new CreateFormCommand(req.Name, req.Description));
            return Results.Created($"/api/forms/{id}", new { id });
        });

        forms.MapPut("/{id:guid}", async (Guid id, UpdateFormRequest req, ISender sender) =>
        {
            await sender.Send(new UpdateFormCommand(id, req.Name, req.Description));
            return Results.NoContent();
        });

        forms.MapDelete("/{id:guid}", async (Guid id, ISender sender) =>
        {
            await sender.Send(new DeleteFormCommand(id));
            return Results.NoContent();
        });

        forms.MapPost("/{id:guid}/publish", async (Guid id, ISender sender) =>
        {
            await sender.Send(new PublishFormCommand(id));
            return Results.NoContent();
        });

        forms.MapPost("/{id:guid}/archive", async (Guid id, ISender sender) =>
        {
            await sender.Send(new ArchiveFormCommand(id));
            return Results.NoContent();
        });

        // ---- Designer ----
        var designer = app.MapGroup("/api/forms/{id:guid}/designer").WithTags("Designer");
        designer.MapGet("/", async (Guid id, ISender sender)
            => Results.Ok(await sender.Send(new GetFormDesignerQuery(id))));
        designer.MapPut("/", async (Guid id, FormDesignerDto design, ISender sender) =>
        {
            await sender.Send(new SaveFormDesignerCommand(design with { Id = id }));
            return Results.NoContent();
        });

        // ---- Field mapping ----
        var mapping = app.MapGroup("/api/forms/{id:guid}/mappings").WithTags("Mappings");
        mapping.MapGet("/", async (Guid id, ISender sender)
            => Results.Ok(await sender.Send(new GetFieldMappingsQuery(id))));
        mapping.MapGet("/fields", async (Guid id, ISender sender)
            => Results.Ok(await sender.Send(new GetMappableFieldsQuery(id))));
        mapping.MapPut("/", async (Guid id, List<MappingItem> items, ISender sender) =>
        {
            await sender.Send(new SaveFieldMappingsCommand(id, items));
            return Results.NoContent();
        });

        // ---- Database metadata ----
        var meta = app.MapGroup("/api/metadata").WithTags("Metadata");
        meta.MapGet("/tables", async (ISender sender)
            => Results.Ok(await sender.Send(new GetTablesQuery())));
        meta.MapGet("/tables/{table}/columns", async (string table, ISender sender)
            => Results.Ok(await sender.Send(new GetColumnsQuery(table))));

        // ---- Submissions ----
        var subs = app.MapGroup("/api").WithTags("Submissions");
        subs.MapPost("/f/{slug}/submit", async (string slug, Dictionary<Guid, string?> values, ISender sender) =>
        {
            var id = await sender.Send(new SubmitFormCommand(slug, values));
            return Results.Ok(new { id });
        });
        subs.MapGet("/forms/{id:guid}/submissions", async (Guid id, ISender sender)
            => Results.Ok(await sender.Send(new GetSubmissionsQuery(id))));
        subs.MapGet("/submissions/{id:guid}", async (Guid id, ISender sender)
            => Results.Ok(await sender.Send(new GetSubmissionQuery(id))));

        return app;
    }

    public record CreateFormRequest(string Name, string? Description);
    public record UpdateFormRequest(string Name, string? Description);
}
