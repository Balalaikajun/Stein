using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.DTOs.User;
using Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;


namespace Application.Services;

public class AuthService: IAuthService
{
    private readonly IApplicationDbContext _context;
    private readonly string _secretKey;
    private readonly int _expiryMinutes;
    private readonly string _issuer;
    private readonly string _audience;
    

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
        
        if(!BCrypt.Net.BCrypt.Verify(userPostDto.Password, user.HashedPassword))
            throw new WrongPasswordException("Passwords do not match");

        return GenerateJwtToken(user.Id);
    }
    
    private string GenerateJwtToken(int id)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, id.ToString()),
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_secretKey)
        );
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(_expiryMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}