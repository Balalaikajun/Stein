using Application.DTOs.Department;
using AutoMapper;
using Domain.Entities;

namespace Infrastructure.Mapping;

public class DepartmentProfile: Profile
{
    public DepartmentProfile()
    {
        CreateMap<Department, DepartmentGetDto>();
    }
}