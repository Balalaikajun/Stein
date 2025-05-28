using Application.DTOs.User;
using Application.Interfaces;
using Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }
    
    [HttpPost]
    public async Task<ActionResult<string>> Login(UserPostDto userPostDto)
    {
        try
        {
            var token = await _authService.LoginAsync(userPostDto);
            
            return Ok(token);
        }
        catch (NotFoundException ex)
        {
            return BadRequest("Неверный логин или пароль");
        }
        catch (WrongPasswordException ex)
        {
            return BadRequest("Неверный логин или пароль");
        }
    }
}