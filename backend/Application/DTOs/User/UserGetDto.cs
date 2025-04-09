using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.User;

public record UserGetDto(
    int Id,
    
    [property: Required(ErrorMessage = "Username is required")]
    [property: MaxLength(50, ErrorMessage = "Username cannot be longer than 50 characters")]
    string Login);