using Application.DTOs.User;

namespace Application.Interfaces;

public interface IAuthService
{
    Task<string> LoginAsync(UserPostDto userPostDto);
}