using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.User;

public record UserPostDto(
    [Required(ErrorMessage = "Username is required")]
    [MaxLength(50, ErrorMessage = "Username cannot be longer than 50 characters")]
    string Login,

    [Required(ErrorMessage = "Password is required")]
    [MaxLength(50, ErrorMessage = "Password cannot be longer than 50 characters")]
    string Password
);