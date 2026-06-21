using FormBuilder.Application;
using FormBuilder.Infrastructure;
using FormBuilder.Web;
using FormBuilder.Infrastructure.Persistence;
using FormBuilder.Web.Components;
using FormBuilder.Web.Endpoints;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Blazor Server (Interactive Server) + MudBlazor
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddMudServices();

// Form Designer state services (per-circuit drag/canvas state, shared layout factory).
builder.Services.AddSingleton<FormBuilder.Web.Services.LayoutDesignerService>();
builder.Services.AddScoped<FormBuilder.Web.Services.DragDropStateService>();
builder.Services.AddScoped<FormBuilder.Web.Services.CanvasStateService>();

// Clean Architecture layers
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Ensure the LocalDB instance is running, then migrate and seed on startup.
LocalDbStarter.EnsureStarted(
    app.Configuration.GetConnectionString("DefaultConnection"),
    app.Logger);

// A freshly-started LocalDB instance attaches its databases lazily, so EF's
// "does the database exist?" probe can briefly report false and try to CREATE an
// existing DB (SQL error 1801). Retry the init a few times while it warms up.
for (var attempt = 1; ; attempt++)
{
    try
    {
        await using var scope = app.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.MigrateAsync();
        await SeedData.SeedAsync(db, app.Configuration);
        break;
    }
    catch (Exception ex) when (attempt < 5)
    {
        app.Logger.LogWarning(ex, "Database init attempt {Attempt} failed; retrying after warm-up.", attempt);
        await Task.Delay(TimeSpan.FromSeconds(2));
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseMiddleware<FormBuilder.Web.Endpoints.ApiExceptionMiddleware>();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Minimal API endpoints (Forms, Designer, Mappings, Submissions, Publishing, Metadata).
app.MapFormBuilderApi();

app.Run();
