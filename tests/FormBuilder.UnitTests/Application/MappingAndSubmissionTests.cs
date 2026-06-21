using FluentAssertions;
using FormBuilder.Application.Features.Mappings;
using FormBuilder.Application.Features.Submissions;
using FormBuilder.Domain.Entities;
using FormBuilder.Domain.Enums;
using FormBuilder.Domain.Exceptions;
using FormBuilder.Domain.Interfaces;
using Moq;
using Xunit;

namespace FormBuilder.UnitTests.Application;

public class MappingAndSubmissionTests
{
    private readonly Mock<IFieldMappingRepository> _mappings = new();
    private readonly Mock<ITableMetadataService> _metadata = new();
    private readonly Mock<IUnitOfWork> _uow = new();

    [Fact]
    public async Task SaveMappings_rejects_duplicate_field()
    {
        var fieldId = Guid.NewGuid();
        _metadata.Setup(m => m.ColumnExistsAsync(It.IsAny<string>(), It.IsAny<string>(), null, It.IsAny<CancellationToken>()))
                 .ReturnsAsync(true);

        var handler = new SaveFieldMappingsCommandHandler(_mappings.Object, _metadata.Object, _uow.Object);
        var cmd = new SaveFieldMappingsCommand(Guid.NewGuid(), new[]
        {
            new MappingItem(fieldId, "Customers", "FullName"),
            new MappingItem(fieldId, "Customers", "Email")
        });

        var act = () => handler.Handle(cmd, default);
        await act.Should().ThrowAsync<DomainException>().WithMessage("*only be mapped to one*");
    }

    [Fact]
    public async Task SaveMappings_rejects_missing_column()
    {
        _metadata.Setup(m => m.ColumnExistsAsync("Customers", "Nope", null, It.IsAny<CancellationToken>()))
                 .ReturnsAsync(false);

        var handler = new SaveFieldMappingsCommandHandler(_mappings.Object, _metadata.Object, _uow.Object);
        var cmd = new SaveFieldMappingsCommand(Guid.NewGuid(), new[]
        {
            new MappingItem(Guid.NewGuid(), "Customers", "Nope")
        });

        var act = () => handler.Handle(cmd, default);
        await act.Should().ThrowAsync<DomainException>().WithMessage("*does not exist*");
    }

    [Fact]
    public async Task SaveMappings_persists_valid_mappings()
    {
        _metadata.Setup(m => m.ColumnExistsAsync(It.IsAny<string>(), It.IsAny<string>(), null, It.IsAny<CancellationToken>()))
                 .ReturnsAsync(true);

        var handler = new SaveFieldMappingsCommandHandler(_mappings.Object, _metadata.Object, _uow.Object);
        var formId = Guid.NewGuid();
        await handler.Handle(new SaveFieldMappingsCommand(formId, new[]
        {
            new MappingItem(Guid.NewGuid(), "Customers", "FullName")
        }), default);

        _mappings.Verify(r => r.ReplaceForFormAsync(formId, It.IsAny<IEnumerable<FieldMapping>>(), It.IsAny<CancellationToken>()), Times.Once);
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SubmitForm_enforces_required_fields()
    {
        var form = BuildPublishedFormWithRequiredField(out var requiredFieldId);
        var forms = new Mock<IFormRepository>();
        forms.Setup(r => r.GetPublishedBySlugAsync("contact", It.IsAny<CancellationToken>())).ReturnsAsync(form);
        var subs = new Mock<IFormSubmissionRepository>();

        var handler = new SubmitFormCommandHandler(forms.Object, subs.Object, _uow.Object);

        var act = () => handler.Handle(
            new SubmitFormCommand("contact", new Dictionary<Guid, string?>()), default);

        await act.Should().ThrowAsync<DomainException>().WithMessage("*required*");
    }

    [Fact]
    public async Task SubmitForm_saves_when_required_provided()
    {
        var form = BuildPublishedFormWithRequiredField(out var requiredFieldId);
        var forms = new Mock<IFormRepository>();
        forms.Setup(r => r.GetPublishedBySlugAsync("contact", It.IsAny<CancellationToken>())).ReturnsAsync(form);
        var subs = new Mock<IFormSubmissionRepository>();

        var handler = new SubmitFormCommandHandler(forms.Object, subs.Object, _uow.Object);
        var id = await handler.Handle(new SubmitFormCommand("contact",
            new Dictionary<Guid, string?> { [requiredFieldId] = "Jane" }), default);

        id.Should().NotBeEmpty();
        subs.Verify(s => s.AddAsync(It.IsAny<FormSubmission>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    private static Form BuildPublishedFormWithRequiredField(out Guid fieldId)
    {
        var form = Form.Create("Contact", null, "contact");
        var page = form.Pages.First();
        var row = page.AddRow(0);
        var col = row.AddColumn(12, 0);
        var field = new FormField("full_name", "Full Name", FieldType.Text, 0);
        field.Configure("Full Name", required: true, null, null, null, null, null);
        col.AddField(field);
        form.Publish();
        fieldId = field.Id;
        return form;
    }
}
