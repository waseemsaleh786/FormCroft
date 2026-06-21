using FormBuilder.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FormBuilder.Infrastructure.Persistence.Configurations;

public class FormConfiguration : IEntityTypeConfiguration<Form>
{
    public void Configure(EntityTypeBuilder<Form> b)
    {
        b.ToTable("Forms");
        b.HasKey(f => f.Id);
        b.Property(f => f.Id).ValueGeneratedNever();

        b.Property(f => f.Name).IsRequired().HasMaxLength(200);
        b.Property(f => f.Description).HasMaxLength(1000);
        b.Property(f => f.Slug).IsRequired().HasMaxLength(250);
        b.Property(f => f.Status).HasConversion<int>();

        b.HasIndex(f => f.Slug).IsUnique();
        b.HasIndex(f => f.Status);

        b.HasMany(f => f.Pages)
            .WithOne()
            .HasForeignKey(p => p.FormId)
            .OnDelete(DeleteBehavior.ClientCascade);
        b.Metadata.FindNavigation(nameof(Form.Pages))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        // Soft delete: deleted forms are excluded from all queries.
        b.HasQueryFilter(f => !f.IsDeleted);
    }
}

public class FormPageConfiguration : IEntityTypeConfiguration<FormPage>
{
    public void Configure(EntityTypeBuilder<FormPage> b)
    {
        b.ToTable("FormPages");
        b.HasKey(p => p.Id);
        b.Property(p => p.Id).ValueGeneratedNever();

        b.Property(p => p.Name).IsRequired().HasMaxLength(200);
        b.HasIndex(p => new { p.FormId, p.Order });

        b.HasMany(p => p.Rows)
            .WithOne()
            .HasForeignKey(r => r.PageId)
            .OnDelete(DeleteBehavior.ClientCascade);
        b.Metadata.FindNavigation(nameof(FormPage.Rows))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}

public class FormRowConfiguration : IEntityTypeConfiguration<FormRow>
{
    public void Configure(EntityTypeBuilder<FormRow> b)
    {
        b.ToTable("FormRows");
        b.HasKey(r => r.Id);
        b.Property(r => r.Id).ValueGeneratedNever();

        b.HasIndex(r => new { r.PageId, r.Order });
        b.HasIndex(r => r.ParentColumnId);

        b.HasMany(r => r.Columns)
            .WithOne()
            .HasForeignKey(c => c.RowId)
            .OnDelete(DeleteBehavior.ClientCascade);
        b.Metadata.FindNavigation(nameof(FormRow.Columns))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}

public class FormColumnConfiguration : IEntityTypeConfiguration<FormColumn>
{
    public void Configure(EntityTypeBuilder<FormColumn> b)
    {
        b.ToTable("FormColumns");
        b.HasKey(c => c.Id);
        b.Property(c => c.Id).ValueGeneratedNever();

        b.Property(c => c.Width).IsRequired();
        b.HasIndex(c => new { c.RowId, c.Order });

        b.HasMany(c => c.Fields)
            .WithOne()
            .HasForeignKey(f => f.ColumnId)
            .OnDelete(DeleteBehavior.ClientCascade);
        b.Metadata.FindNavigation(nameof(FormColumn.Fields))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        // Nested sub-rows. ClientCascade because SQL Server rejects the cascade
        // cycle Column → Row → Column; EF deletes the loaded sub-graph instead.
        b.HasMany(c => c.SubRows)
            .WithOne()
            .HasForeignKey(r => r.ParentColumnId)
            .OnDelete(DeleteBehavior.ClientCascade);
        b.Metadata.FindNavigation(nameof(FormColumn.SubRows))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}

public class FormFieldConfiguration : IEntityTypeConfiguration<FormField>
{
    public void Configure(EntityTypeBuilder<FormField> b)
    {
        b.ToTable("FormFields");
        b.HasKey(f => f.Id);
        b.Property(f => f.Id).ValueGeneratedNever();

        b.Property(f => f.Name).IsRequired().HasMaxLength(200);
        b.Property(f => f.Label).HasMaxLength(250);
        b.Property(f => f.Type).HasConversion<int>();
        b.Property(f => f.Placeholder).HasMaxLength(500);
        b.Property(f => f.HelpText).HasMaxLength(1000);
        b.Property(f => f.DefaultValue).HasMaxLength(1000);
        b.Property(f => f.ValidationJson).HasColumnType("nvarchar(max)");
        b.Property(f => f.SettingsJson).HasColumnType("nvarchar(max)");

        b.HasIndex(f => new { f.ColumnId, f.Order });
    }
}
