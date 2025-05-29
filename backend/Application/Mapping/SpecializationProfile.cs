using Application.DTOs.Specialization;
using AutoMapper;
using Domain.Entities;

namespace Application.Mapping;

public class SpecializationProfile : Profile
{
    public SpecializationProfile()
    {
        CreateMap<Specialization, SpecializationGetDto>()
            .ForMember(dest => dest.DepartmentTitle,
                opt => opt.MapFrom(src => src.Department.Title));

        CreateMap<SpecializationPostDto, Specialization>();
    }
}