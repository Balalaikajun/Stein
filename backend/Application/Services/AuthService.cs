using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.DTOs.User;
using Application.Interfaces;
using Domain.Exceptions;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Application.Services;

public class AuthService : IAuthService
{
    private readonly string _audience;
    private readonly IApplicationDbContext _context;
    private readonly int _expiryMinutes;
    private readonly string _issuer;
    private readonly string _secretKey;


    public AuthService(IApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _secretKey = configuration["Jwt:Key"] ??
                     throw new BadConfigurationException("Auth Secret Key is missing in configuration");
        _expiryMinutes = int.Parse(configuration["Jwt:ExpiryMinutes"] ??
                                   throw new BadConfigurationException("ExpiryMinutes is missing in configuration"));
        _issuer = configuration["Jwt:Issuer"];
        _audience = configuration["Jwt:Audience"];
    }

    public async Task<string> LoginAsync(UserPostDto userPostDto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Login == userPostDto.Login) ??
                   throw new NotFoundException("User was not found");

        if (!BCrypt.Net.BCrypt.Verify(userPostDto.Password, user.HashedPassword))
            throw new WrongPasswordException("Passwords do not match");

        return GenerateJwtToken(user.Id);
    }

    private string GenerateJwtToken(int id)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, id.ToString())
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_secretKey)
        );
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            _issuer,
            _audience,
            claims,
            expires: DateTime.Now.AddYears(100),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}