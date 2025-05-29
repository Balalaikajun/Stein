using Application.DTOs.Department;
using AutoMapper;
using Domain.Entities;

namespace Application.Mapping;

public class DepartmentProfile : Profile
{
    public DepartmentProfile()
    {
        CreateMap<Department, DepartmentGetDto>();

        CreateMap<DepartmentPostDto, Department>();
    }
}