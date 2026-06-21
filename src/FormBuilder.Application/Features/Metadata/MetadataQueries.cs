using FormBuilder.Application.DTOs;
using FormBuilder.Domain.Interfaces;
using MediatR;

namespace FormBuilder.Application.Features.Metadata;

public sealed record GetTablesQuery(string? ConnectionString = null)
    : IRequest<IReadOnlyList<DatabaseTableDto>>;

public sealed class GetTablesQueryHandler
    : IRequestHandler<GetTablesQuery, IReadOnlyList<DatabaseTableDto>>
{
    private readonly ITableMetadataService _metadata;
    public GetTablesQueryHandler(ITableMetadataService metadata) => _metadata = metadata;

    public async Task<IReadOnlyList<DatabaseTableDto>> Handle(GetTablesQuery request, CancellationToken ct)
    {
        var tables = await _metadata.GetTablesAsync(request.ConnectionString, ct);
        return tables.Select(t => new DatabaseTableDto(t.Schema, t.Name, t.FullName)).ToList();
    }
}

public sealed record GetColumnsQuery(string TableName, string? ConnectionString = null)
    : IRequest<IReadOnlyList<DatabaseColumnDto>>;

public sealed class GetColumnsQueryHandler
    : IRequestHandler<GetColumnsQuery, IReadOnlyList<DatabaseColumnDto>>
{
    private readonly ITableMetadataService _metadata;
    public GetColumnsQueryHandler(ITableMetadataService metadata) => _metadata = metadata;

    public async Task<IReadOnlyList<DatabaseColumnDto>> Handle(GetColumnsQuery request, CancellationToken ct)
    {
        var columns = await _metadata.GetColumnsAsync(request.TableName, request.ConnectionString, ct);
        return columns
            .Select(c => new DatabaseColumnDto(c.Name, c.DataType, c.IsNullable, c.MaxLength, c.IsPrimaryKey))
            .ToList();
    }
}
