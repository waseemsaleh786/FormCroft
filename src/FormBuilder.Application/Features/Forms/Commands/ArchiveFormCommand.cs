using FormBuilder.Application.Common.Exceptions;
using FormBuilder.Domain.Interfaces;
using MediatR;

namespace FormBuilder.Application.Features.Forms.Commands;

public sealed record ArchiveFormCommand(Guid Id) : IRequest;

public sealed class ArchiveFormCommandHandler : IRequestHandler<ArchiveFormCommand>
{
    private readonly IFormRepository _forms;
    private readonly IUnitOfWork _uow;

    public ArchiveFormCommandHandler(IFormRepository forms, IUnitOfWork uow)
    {
        _forms = forms;
        _uow = uow;
    }

    public async Task Handle(ArchiveFormCommand request, CancellationToken ct)
    {
        var form = await _forms.GetAsync(request.Id, ct)
                   ?? throw new NotFoundException(nameof(Domain.Entities.Form), request.Id);

        form.Archive();
        await _uow.SaveChangesAsync(ct);
    }
}
