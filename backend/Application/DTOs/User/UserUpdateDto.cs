using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.User;

public record UserUpdateDto(
    [property: Required(ErrorMessage = "Id is required")]
    int Id,
    
    [property: MaxLength(50, ErrorMessage = "Username cannot be longer than 50 characters")]
    string? Login,

    [property: MaxLength(50, ErrorMessage = "Password cannot be longer than 50 characters")]
    string? Password);