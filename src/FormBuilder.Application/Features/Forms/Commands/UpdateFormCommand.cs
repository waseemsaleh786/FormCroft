using FluentValidation;
using FormBuilder.Application.Common.Exceptions;
using FormBuilder.Domain.Interfaces;
using MediatR;

namespace FormBuilder.Application.Features.Forms.Commands;

public sealed record UpdateFormCommand(Guid Id, string Name, string? Description) : IRequest;

public sealed class UpdateFormCommandValidator : AbstractValidator<UpdateFormCommand>
{
    public UpdateFormCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).MaximumLength(1000);
    }
}

public sealed class UpdateFormCommandHandler : IRequestHandler<UpdateFormCommand>
{
    private readonly IFormRepository _forms;
    private readonly IUnitOfWork _uow;

    public UpdateFormCommandHandler(IFormRepository forms, IUnitOfWork uow)
    {
        _forms = forms;
        _uow = uow;
    }

    public async Task Handle(UpdateFormCommand request, CancellationToken ct)
    {
        var form = await _forms.GetAsync(request.Id, ct)
                   ?? throw new NotFoundException(nameof(Domain.Entities.Form), request.Id);

        form.Rename(request.Name);
        form.UpdateDescription(request.Description);
        await _uow.SaveChangesAsync(ct);
    }
}
