using FluentAssertions;
using FormBuilder.Application.Features.Forms.Commands;
using FormBuilder.Domain.Entities;
using FormBuilder.Domain.Interfaces;
using Moq;
using Xunit;

namespace FormBuilder.UnitTests.Application;

public class FormCommandHandlerTests
{
    private readonly Mock<IFormRepository> _forms = new();
    private readonly Mock<IUnitOfWork> _uow = new();

    [Fact]
    public async Task CreateForm_generates_unique_slug_and_persists()
    {
        // First slug taken, second free → expect "contact-form-2".
        _forms.SetupSequence(r => r.SlugExistsAsync(It.IsAny<string>(), null, It.IsAny<CancellationToken>()))
              .ReturnsAsync(true)
              .ReturnsAsync(false);

        Form? added = null;
        _forms.Setup(r => r.AddAsync(It.IsAny<Form>(), It.IsAny<CancellationToken>()))
              .Callback<Form, CancellationToken>((f, _) => added = f)
              .Returns(Task.CompletedTask);

        var handler = new CreateFormCommandHandler(_forms.Object, _uow.Object);
        var id = await handler.Handle(new CreateFormCommand("Contact Form", "desc"), default);

        id.Should().NotBeEmpty();
        added.Should().NotBeNull();
        added!.Slug.Should().Be("contact-form-2");
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteForm_soft_deletes()
    {
        var form = Form.Create("X", null, "x");
        _forms.Setup(r => r.GetAsync(form.Id, It.IsAny<CancellationToken>())).ReturnsAsync(form);

        var handler = new DeleteFormCommandHandler(_forms.Object, _uow.Object);
        await handler.Handle(new DeleteFormCommand(form.Id), default);

        form.IsDeleted.Should().BeTrue();
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task PublishForm_sets_published()
    {
        var form = Form.Create("X", null, "x");
        _forms.Setup(r => r.GetAsync(form.Id, It.IsAny<CancellationToken>())).ReturnsAsync(form);

        var handler = new PublishFormCommandHandler(_forms.Object, _uow.Object);
        await handler.Handle(new PublishFormCommand(form.Id), default);

        form.Status.Should().Be(FormBuilder.Domain.Enums.FormStatus.Published);
    }
}
