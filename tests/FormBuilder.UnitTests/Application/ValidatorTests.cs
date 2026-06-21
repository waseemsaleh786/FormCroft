using FluentAssertions;
using FluentValidation.TestHelper;
using FormBuilder.Application.DTOs;
using FormBuilder.Application.Features.Designer.Commands;
using FormBuilder.Application.Features.Forms.Commands;
using Xunit;

namespace FormBuilder.UnitTests.Application;

public class ValidatorTests
{
    [Fact]
    public void CreateForm_requires_name()
    {
        var validator = new CreateFormCommandValidator();
        validator.TestValidate(new CreateFormCommand("", null))
            .ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void CreateForm_accepts_valid_name()
    {
        var validator = new CreateFormCommandValidator();
        validator.TestValidate(new CreateFormCommand("Survey", null))
            .ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void SaveDesigner_rejects_row_columns_over_12()
    {
        var validator = new SaveFormDesignerCommandValidator();
        var design = new FormDesignerDto
        {
            Id = Guid.NewGuid(),
            Name = "F",
            Pages = new()
            {
                new PageDto
                {
                    Name = "P",
                    Rows = new()
                    {
                        new RowDto { Columns = new() { new ColumnDto { Width = 8 }, new ColumnDto { Width = 8 } } }
                    }
                }
            }
        };

        var result = validator.TestValidate(new SaveFormDesignerCommand(design));
        result.Errors.Should().NotBeEmpty();
    }
}
