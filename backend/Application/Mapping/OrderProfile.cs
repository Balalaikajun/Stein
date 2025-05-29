using Application.DTOs.Order;
using AutoMapper;
using Domain.Entities;

namespace Application.Mapping;

public class OrderProfile : Profile
{
    public OrderProfile()
    {
        CreateMap<Order, OrderGetDto>()
            .ForMember(dest => dest.StudentFullName,
                opt =>
                    opt.MapFrom(src =>
                        src.Student.Surname + " " +
                        src.Student.Name.Substring(0, 1) + "." +
                        src.Student.Patronymic.Substring(0, 1) + "."))
            .ForMember(d => d.GroupToAcronym,
                o => o.MapFrom(src => src.ToGroup.Acronym))
            .ForMember(d => d.GroupFromAcronym,
                o => o.MapFrom(src => src.FromGroup.Acronym));

        CreateMap<OrderPostDto, Order>();
    }
}