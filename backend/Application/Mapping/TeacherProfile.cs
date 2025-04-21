using Application.DTOs.Teacher;
using AutoMapper;
using Domain.Entities;

namespace Infrastructure.Mapping;

public class TeacherProfile:Profile
{
    public TeacherProfile()
    {
        CreateMap<Teacher, TeacherGetDto>();
    }
}