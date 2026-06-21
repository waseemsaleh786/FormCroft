using FormBuilder.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FormBuilder.Infrastructure.Persistence.Configurations;

public class FieldMappingConfiguration : IEntityTypeConfiguration<FieldMapping>
{
    public void Configure(EntityTypeBuilder<FieldMapping> b)
    {
        b.ToTable("FieldMappings");
        b.HasKey(m => m.Id);

        b.Property(m => m.TableName).IsRequired().HasMaxLength(256);
        b.Property(m => m.ColumnName).IsRequired().HasMaxLength(256);

        // Enforce the "one form field → one column" invariant.
        b.HasIndex(m => new { m.FormId, m.FormFieldId }).IsUnique();
        b.HasIndex(m => m.FormId);
    }
}

public class FormSubmissionConfiguration : IEntityTypeConfiguration<FormSubmission>
{
    public void Configure(EntityTypeBuilder<FormSubmission> b)
    {
        b.ToTable("FormSubmissions");
        b.HasKey(s => s.Id);

        b.HasIndex(s => s.FormId);
        b.HasIndex(s => s.SubmittedAt);

        b.HasMany(s => s.Values)
            .WithOne()
            .HasForeignKey(v => v.SubmissionId)
            .OnDelete(DeleteBehavior.Cascade);
        b.Metadata.FindNavigation(nameof(FormSubmission.Values))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}

public class FormSubmissionValueConfiguration : IEntityTypeConfiguration<FormSubmissionValue>
{
    public void Configure(EntityTypeBuilder<FormSubmissionValue> b)
    {
        b.ToTable("FormSubmissionValues");
        b.HasKey(v => v.Id);

        b.Property(v => v.Value).HasColumnType("nvarchar(max)");
        b.HasIndex(v => v.FormFieldId);
    }
}

public class DatabaseConnectionConfiguration : IEntityTypeConfiguration<DatabaseConnection>
{
    public void Configure(EntityTypeBuilder<DatabaseConnection> b)
    {
        b.ToTable("DatabaseConnections");
        b.HasKey(c => c.Id);

        b.Property(c => c.Name).IsRequired().HasMaxLength(200);
        b.Property(c => c.ConnectionString).IsRequired().HasMaxLength(1000);
        b.Property(c => c.Provider).IsRequired().HasMaxLength(50);
        b.HasIndex(c => c.Name).IsUnique();
    }
}
