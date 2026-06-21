using FormBuilder.Application.Common.Exceptions;
using FormBuilder.Domain.Interfaces;
using MediatR;

namespace FormBuilder.Application.Features.Forms.Commands;

/// <summary>Soft-deletes a form (sets IsDeleted; row is retained).</summary>
public sealed record DeleteFormCommand(Guid Id) : IRequest;

public sealed class DeleteFormCommandHandler : IRequestHandler<DeleteFormCommand>
{
    private readonly IFormRepository _forms;
    private readonly IUnitOfWork _uow;

    public DeleteFormCommandHandler(IFormRepository forms, IUnitOfWork uow)
    {
        _forms = forms;
        _uow = uow;
    }

    public async Task Handle(DeleteFormCommand request, CancellationToken ct)
    {
        var form = await _forms.GetAsync(request.Id, ct)
                   ?? throw new NotFoundException(nameof(Domain.Entities.Form), request.Id);

        form.SoftDelete();
        await _uow.SaveChangesAsync(ct);
    }
}
