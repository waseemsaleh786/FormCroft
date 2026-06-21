using FluentAssertions;
using FormBuilder.Application.Common;
using FormBuilder.Domain.Entities;
using FormBuilder.Domain.Exceptions;
using Xunit;

namespace FormBuilder.UnitTests.Domain;

public class LayoutAndSlugTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(13)]
    public void Column_width_must_be_between_1_and_12(int width)
    {
        var act = () => new FormColumn(width, 0);
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Column_preserves_supplied_id()
    {
        var id = Guid.NewGuid();
        var column = new FormColumn(6, 0, id);
        column.Id.Should().Be(id);
    }

    [Theory]
    [InlineData("Contact Form", "contact-form")]
    [InlineData("  Multiple   Spaces  ", "multiple-spaces")]
    [InlineData("Special!@#Chars", "specialchars")]
    public void SlugGenerator_produces_url_friendly_slugs(string input, string expected)
    {
        SlugGenerator.Generate(input).Should().Be(expected);
    }

    [Fact]
    public void SlugGenerator_falls_back_for_empty_input()
    {
        SlugGenerator.Generate("   ").Should().NotBeNullOrWhiteSpace();
    }
}
