using Application.DTOs.User;
using AutoMapper;
using Domain.Entities;

namespace Application.Mapping;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserGetDto>();
    }
}