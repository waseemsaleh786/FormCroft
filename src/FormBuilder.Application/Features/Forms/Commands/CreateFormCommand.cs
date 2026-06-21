using FluentValidation;
using FormBuilder.Application.Common;
using FormBuilder.Domain.Entities;
using FormBuilder.Domain.Interfaces;
using MediatR;

namespace FormBuilder.Application.Features.Forms.Commands;

public sealed record CreateFormCommand(string Name, string? Description) : IRequest<Guid>;

public sealed class CreateFormCommandValidator : AbstractValidator<CreateFormCommand>
{
    public CreateFormCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Form name is required.")
            .MaximumLength(200);
        RuleFor(x => x.Description).MaximumLength(1000);
    }
}

public sealed class CreateFormCommandHandler : IRequestHandler<CreateFormCommand, Guid>
{
    private readonly IFormRepository _forms;
    private readonly IUnitOfWork _uow;

    public CreateFormCommandHandler(IFormRepository forms, IUnitOfWork uow)
    {
        _forms = forms;
        _uow = uow;
    }

    public async Task<Guid> Handle(CreateFormCommand request, CancellationToken ct)
    {
        var slug = await GenerateUniqueSlugAsync(request.Name, ct);
        var form = Form.Create(request.Name, request.Description, slug);
        await _forms.AddAsync(form, ct);
        await _uow.SaveChangesAsync(ct);
        return form.Id;
    }

    private async Task<string> GenerateUniqueSlugAsync(string name, CancellationToken ct)
    {
        var baseSlug = SlugGenerator.Generate(name);
        var slug = baseSlug;
        var suffix = 1;
        while (await _forms.SlugExistsAsync(slug, null, ct))
            slug = $"{baseSlug}-{++suffix}";
        return slug;
    }
}
