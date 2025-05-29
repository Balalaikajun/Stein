using Application.DTOs.AcademicPerformance;
using AutoMapper;
using Domain.Entities;

namespace Application.Mapping;

public class AcademicPerformanceProfile : Profile
{
    public AcademicPerformanceProfile()
    {
        CreateMap<AcademicPerformance, AcademicPerformanceGetDto>();
    }
}