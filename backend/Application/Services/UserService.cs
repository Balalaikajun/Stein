using Application.DTOs.User;
using Application.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class UserService : IUserService
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public UserService(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task AddUserAsync(UserPostDto userPostDto)
    {
        _context.Users.Add(new User
        {
            Login = userPostDto.Login,
            HashedPassword = BCrypt.Net.BCrypt.HashPassword(userPostDto.Password)
        });

        await _context.SaveChangesAsync();
    }

    public async Task DeleteUserAsync(int userId)
    {
        var user = await _context.Users.FirstOrDefaultAsync(w => w.Id == userId);

        if (user == null)
            return;

        _context.Users.Remove(user);

        await _context.SaveChangesAsync();
    }

    public async Task<List<UserGetDto>> GetUsersAsync()
    {
        return await _context.Users.ProjectTo<UserGetDto>(_mapper.ConfigurationProvider).ToListAsync();
    }

    public async Task UpdateUserAsync(UserUpdateDto userUpdateDto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userUpdateDto.Id) ??
                   throw new NotFoundException($"User with id:{userUpdateDto.Id}, was not found");

        if (userUpdateDto.Login != null && userUpdateDto.Login != user.Login)
            user.Login = userUpdateDto.Login;

        if (userUpdateDto.Password != null)
            user.HashedPassword = BCrypt.Net.BCrypt.HashPassword(userUpdateDto.Password);

        await _context.SaveChangesAsync();
    }
}