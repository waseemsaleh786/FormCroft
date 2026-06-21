using AutoMapper;
using FormBuilder.Application.DTOs;
using FormBuilder.Domain.Entities;

namespace FormBuilder.Application.Common.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Form, FormSummaryDto>()
            .ForMember(d => d.PageCount, o => o.MapFrom(s => s.Pages.Count))
            .ForMember(d => d.FieldCount, o => o.MapFrom(s =>
                s.Pages.SelectMany(p => p.Rows)
                       .SelectMany(r => r.Columns)
                       .SelectMany(c => c.Fields).Count()))
            .ForMember(d => d.SubmissionCount, o => o.Ignore());

        CreateMap<FormField, MappableFieldDto>();

        CreateMap<DatabaseTable, DatabaseTableDto>()
            .ConstructUsing(t => new DatabaseTableDto(t.Schema, t.Name, t.FullName));

        CreateMap<DatabaseColumn, DatabaseColumnDto>()
            .ConstructUsing(c => new DatabaseColumnDto(c.Name, c.DataType, c.IsNullable, c.MaxLength, c.IsPrimaryKey));
    }
}
