using Application.DTOs.Student;
using AutoMapper;
using Domain.Entities;

namespace Application.Mapping;

public class StudentProfile : Profile
{
    public StudentProfile()
    {
        CreateMap<Student, StudentGetDto>()
            .ForMember(d => d.GroupAcronym,
                o => o.MapFrom(s => s.Group.Acronym));
        CreateMap<StudentPostDto, Student>();
    }
}