using FluentAssertions;
using FormBuilder.Domain.Entities;
using FormBuilder.Domain.Enums;
using FormBuilder.Domain.Events;
using FormBuilder.Domain.Exceptions;
using Xunit;

namespace FormBuilder.UnitTests.Domain;

public class FormTests
{
    private static Form NewForm() => Form.Create("Contact", "desc", "contact");

    [Fact]
    public void Create_starts_as_draft_with_one_page_and_raises_event()
    {
        var form = NewForm();

        form.Status.Should().Be(FormStatus.Draft);
        form.Pages.Should().HaveCount(1);
        form.DomainEvents.Should().ContainSingle(e => e is FormCreatedEvent);
    }

    [Fact]
    public void Create_with_empty_name_throws()
    {
        var act = () => Form.Create("  ", null, "x");
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Publish_moves_draft_to_published_and_raises_event()
    {
        var form = NewForm();

        form.Publish();

        form.Status.Should().Be(FormStatus.Published);
        form.DomainEvents.Should().Contain(e => e is FormPublishedEvent);
    }

    [Fact]
    public void Published_form_is_read_only()
    {
        var form = NewForm();
        form.Publish();

        var act = () => form.Rename("New name");

        act.Should().Throw<DomainException>().WithMessage("*read-only*");
    }

    [Fact]
    public void Archive_requires_published_state()
    {
        var form = NewForm();

        var act = () => form.Archive();

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Archived_form_cannot_be_published()
    {
        var form = NewForm();
        form.Publish();
        form.Archive();

        var act = () => form.Publish();

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void RevertToDraft_allows_editing_again()
    {
        var form = NewForm();
        form.Publish();

        form.RevertToDraft();
        form.Rename("Edited");

        form.Status.Should().Be(FormStatus.Draft);
        form.Name.Should().Be("Edited");
    }

    [Fact]
    public void Cannot_remove_the_only_page()
    {
        var form = NewForm();
        var only = form.Pages.First();

        var act = () => form.RemovePage(only.Id);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void SoftDelete_sets_flag()
    {
        var form = NewForm();
        form.SoftDelete();
        form.IsDeleted.Should().BeTrue();
    }
}
