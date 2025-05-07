using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Application.DTOs.Student;

public record StudentPostDto(
    [Required(ErrorMessage = "Surname is required")]
    [MaxLength(32, ErrorMessage = "Surname cannot be longer than 32 characters")]
    string Surname,
    [Required(ErrorMessage = "Name is required")]
    [MaxLength(32, ErrorMessage = "Name cannot be longer than 32 characters")]
    string Name,
    [Required(ErrorMessage = "Patronymic is required")]
    [MaxLength(32, ErrorMessage = "Patronymic cannot be longer than 32 characters")]
    string Patronymic,
    [Required(ErrorMessage = "Gender is required")]
    Gender Gender,
    [Required(ErrorMessage = "Birthday is required")]
    DateOnly DateOfBirth,
    [Required(ErrorMessage = "IsCitizen is required")]
    bool IsCitizen
    );