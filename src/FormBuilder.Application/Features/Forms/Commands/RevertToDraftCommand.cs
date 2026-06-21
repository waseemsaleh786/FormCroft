using FormBuilder.Application.Common.Exceptions;
using FormBuilder.Domain.Interfaces;
using MediatR;

namespace FormBuilder.Application.Features.Forms.Commands;

/// <summary>Reverts a published form to Draft so it can be edited again.</summary>
public sealed record RevertToDraftCommand(Guid Id) : IRequest;

public sealed class RevertToDraftCommandHandler : IRequestHandler<RevertToDraftCommand>
{
    private readonly IFormRepository _forms;
    private readonly IUnitOfWork _uow;

    public RevertToDraftCommandHandler(IFormRepository forms, IUnitOfWork uow)
    {
        _forms = forms;
        _uow = uow;
    }

    public async Task Handle(RevertToDraftCommand request, CancellationToken ct)
    {
        var form = await _forms.GetAsync(request.Id, ct)
                   ?? throw new NotFoundException(nameof(Domain.Entities.Form), request.Id);

        form.RevertToDraft();
        await _uow.SaveChangesAsync(ct);
    }
}
