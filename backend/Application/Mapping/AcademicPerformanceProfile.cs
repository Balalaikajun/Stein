using Application.DTOs.AcademicPerformance;
using AutoMapper;
using Domain.Entities;

namespace Infrastructure.Mapping;

public class AcademicPerformanceProfile:Profile
{
    public AcademicPerformanceProfile()
    {
        CreateMap<AcademicPerformance, AcademicPerformanceGetDto>();
    }
}