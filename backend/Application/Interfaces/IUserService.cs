using Application.DTOs.User;

namespace Application.Interfaces;

public interface IUserService
{
    Task AddUserAsync(UserPostDto userPostDto);
    Task DeleteUserAsync(int userId);
    Task<List<UserGetDto>> GetUsersAsync();
    Task UpdateUserAsync(UserUpdateDto userUpdateDto);
}