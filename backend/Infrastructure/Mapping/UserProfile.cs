using AutoMapper;
using Domain.Entities;
using Application.DTOs.User;

namespace Infrastructure.Mapping;

public class UserProfile: Profile
{
    public UserProfile()
    {
        CreateMap<User, UserGetDto>();
    }
}