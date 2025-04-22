using Application.DTOs.Group;
using Application.DTOs.Order;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;

namespace Infrastructure.Mapping;

public class OrderProfile: Profile
{
    public OrderProfile()
    {
        CreateMap<Order, OrderGetDto>()
            .ForMember(dest => dest.StudentFullName,
                opt =>
                    opt.MapFrom(src =>
                        src.Student.Surname + " " +
                        src.Student.Name.Substring(0, 1) + "." +
                        src.Student.Patronymic.Substring(0, 1) + "."));

        CreateMap<EnrollmentOrder, OrderGetDto>()
            .IncludeBase<Order, OrderGetDto>()
            .ForMember(d => d.GroupTo,
                o => o.MapFrom(
                    src => new GroupKeyDto(src.ToSpecializationId, src.ToYear, src.ToGroupId)));
        CreateMap<ExpulsionOrder, OrderGetDto>()
            .IncludeBase<Order, OrderGetDto>()
            .ForMember(d => d.GroupFrom,
                o => o.MapFrom(
                    src => new GroupKeyDto(src.FromSpecializationId, src.FromYear, src.FromGroupId)));
        CreateMap<TransferOrder, OrderGetDto>()
            .IncludeBase<Order, OrderGetDto>()
            .ForMember(d => d.GroupTo,
                o => o.MapFrom(
                    src => new GroupKeyDto(src.ToSpecializationId, src.ToYear, src.ToGroupId)))
            .ForMember(d => d.GroupFrom,
                o => o.MapFrom(
                    src => new GroupKeyDto(src.FromSpecializationId, src.FromYear, src.FromGroupId)));
       
        
    }
    
}