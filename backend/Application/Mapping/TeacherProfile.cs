using Application.DTOs.Teacher;
using AutoMapper;
using Domain.Entities;

namespace Application.Mapping;

public class TeacherProfile : Profile
{
    public TeacherProfile()
    {
        CreateMap<Teacher, TeacherGetDto>();

        CreateMap<TeacherPostDto, Teacher>();
    }
}