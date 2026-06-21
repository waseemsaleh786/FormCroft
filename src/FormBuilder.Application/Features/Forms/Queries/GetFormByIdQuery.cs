using AutoMapper;
using FormBuilder.Application.Common.Exceptions;
using FormBuilder.Application.DTOs;
using FormBuilder.Domain.Interfaces;
using MediatR;

namespace FormBuilder.Application.Features.Forms.Queries;

public sealed record GetFormByIdQuery(Guid Id) : IRequest<FormSummaryDto>;

public sealed class GetFormByIdQueryHandler : IRequestHandler<GetFormByIdQuery, FormSummaryDto>
{
    private readonly IFormRepository _forms;
    private readonly IMapper _mapper;

    public GetFormByIdQueryHandler(IFormRepository forms, IMapper mapper)
    {
        _forms = forms;
        _mapper = mapper;
    }

    public async Task<FormSummaryDto> Handle(GetFormByIdQuery request, CancellationToken ct)
    {
        var form = await _forms.GetWithLayoutAsync(request.Id, ct)
                   ?? throw new NotFoundException(nameof(Domain.Entities.Form), request.Id);
        return _mapper.Map<FormSummaryDto>(form);
    }
}
