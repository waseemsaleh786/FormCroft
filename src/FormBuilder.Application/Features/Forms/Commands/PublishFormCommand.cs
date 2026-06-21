using FormBuilder.Application.Common.Exceptions;
using FormBuilder.Domain.Interfaces;
using MediatR;

namespace FormBuilder.Application.Features.Forms.Commands;

public sealed record PublishFormCommand(Guid Id) : IRequest;

public sealed class PublishFormCommandHandler : IRequestHandler<PublishFormCommand>
{
    private readonly IFormRepository _forms;
    private readonly IUnitOfWork _uow;

    public PublishFormCommandHandler(IFormRepository forms, IUnitOfWork uow)
    {
        _forms = forms;
        _uow = uow;
    }

    public async Task Handle(PublishFormCommand request, CancellationToken ct)
    {
        var form = await _forms.GetAsync(request.Id, ct)
                   ?? throw new NotFoundException(nameof(Domain.Entities.Form), request.Id);

        form.Publish();
        await _uow.SaveChangesAsync(ct);
    }
}
