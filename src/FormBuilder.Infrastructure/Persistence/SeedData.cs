using FormBuilder.Application.Common;
using FormBuilder.Domain.Entities;
using FormBuilder.Domain.Enums;
using FormBuilder.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace FormBuilder.Infrastructure.Persistence;

/// <summary>
/// Seeds the database on startup: sample business tables (mapping targets),
/// a default database connection, and a couple of example forms.
/// </summary>
public static class SeedData
{
    public static async Task SeedAsync(AppDbContext db, IConfiguration configuration, CancellationToken ct = default)
    {
        await SeedSampleTargetTablesAsync(db, ct);
        await SeedDefaultConnectionAsync(db, configuration, ct);
        await SeedSampleFormsAsync(db, ct);
        await db.SaveChangesAsync(ct);
    }

    /// <summary>Creates realistic business tables so the mapping screen has real columns to map to.</summary>
    private static async Task SeedSampleTargetTablesAsync(AppDbContext db, CancellationToken ct)
    {
        const string sql = @"
IF OBJECT_ID('dbo.Customers', 'U') IS NULL
CREATE TABLE dbo.Customers (
    Id INT IDENTITY PRIMARY KEY,
    FullName NVARCHAR(200) NOT NULL,
    Email NVARCHAR(256) NULL,
    Phone NVARCHAR(50) NULL,
    Country NVARCHAR(100) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
);

IF OBJECT_ID('dbo.Leads', 'U') IS NULL
CREATE TABLE dbo.Leads (
    Id INT IDENTITY PRIMARY KEY,
    ContactName NVARCHAR(200) NOT NULL,
    Company NVARCHAR(200) NULL,
    Email NVARCHAR(256) NULL,
    Phone NVARCHAR(50) NULL,
    Source NVARCHAR(100) NULL,
    Notes NVARCHAR(MAX) NULL,
    PreferredDate DATE NULL
);

IF OBJECT_ID('dbo.EventRegistrations', 'U') IS NULL
CREATE TABLE dbo.EventRegistrations (
    Id INT IDENTITY PRIMARY KEY,
    AttendeeName NVARCHAR(200) NOT NULL,
    Email NVARCHAR(256) NULL,
    TicketType NVARCHAR(100) NULL,
    Attending BIT NULL
);";
        await db.Database.ExecuteSqlRawAsync(sql, ct);
    }

    private static async Task SeedDefaultConnectionAsync(AppDbContext db, IConfiguration configuration, CancellationToken ct)
    {
        if (await db.DatabaseConnections.AnyAsync(ct)) return;

        var cs = configuration.GetConnectionString("DefaultConnection") ?? "";
        var conn = new DatabaseConnection("Application Database", cs);
        conn.MarkDefault(true);
        await db.DatabaseConnections.AddAsync(conn, ct);
    }

    private static async Task SeedSampleFormsAsync(AppDbContext db, CancellationToken ct)
    {
        if (await db.Forms.IgnoreQueryFilters().AnyAsync(ct)) return;

        db.Forms.Add(BuildContactForm());
        db.Forms.Add(BuildEventRegistrationForm());
        await Task.CompletedTask;
    }

    private static Form BuildContactForm()
    {
        var form = Form.Create("Contact Form", "Collect customer inquiries and feedback", SlugGenerator.Generate("Contact Form"));
        var page = form.Pages.First();
        page.Rename("Personal Information");

        var row1 = page.AddRow(0);
        var col1 = row1.AddColumn(12, 0);
        col1.AddField(NewField("full_name", "Full Name", FieldType.Text, 0, required: true));

        var row2 = page.AddRow(1);
        var col2a = row2.AddColumn(6, 0);
        col2a.AddField(NewField("email", "Email Address", FieldType.Email, 0, required: true));
        var col2b = row2.AddColumn(6, 1);
        col2b.AddField(NewField("phone", "Phone Number", FieldType.Phone, 0));

        var row3 = page.AddRow(2);
        var col3 = row3.AddColumn(12, 0);
        col3.AddField(NewField("message", "Message", FieldType.Textarea, 0));

        return form;
    }

    private static Form BuildEventRegistrationForm()
    {
        var form = Form.Create("Event Registration", "Register attendees for upcoming events", SlugGenerator.Generate("Event Registration"));
        var page = form.Pages.First();
        page.Rename("Registration");

        var row1 = page.AddRow(0);
        row1.AddColumn(12, 0).AddField(NewField("attendee_name", "Attendee Name", FieldType.Text, 0, required: true));

        var row2 = page.AddRow(1);
        row2.AddColumn(6, 0).AddField(NewField("email", "Email", FieldType.Email, 0, required: true));
        row2.AddColumn(6, 1).AddField(NewField("ticket_type", "Ticket Type", FieldType.Dropdown, 0));

        return form;
    }

    private static FormField NewField(string name, string label, FieldType type, int order, bool required = false)
    {
        var field = new FormField(name, label, type, order);
        field.Configure(label, required, placeholder: null, helpText: null, defaultValue: null,
            validationJson: FieldConfigSerializer.Serialize(FieldValidation.Empty),
            settingsJson: FieldConfigSerializer.Serialize(new FieldSettings()));
        return field;
    }
}
