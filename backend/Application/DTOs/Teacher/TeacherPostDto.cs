using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Teacher;

public record TeacherPostDto(
    [Required(ErrorMessage = "Surname is required")]
    [MaxLength(32, ErrorMessage = "Title cannot be longer than 32 characters")]
    string Surname,
    [Required(ErrorMessage = "Name is required")]
    [MaxLength(32, ErrorMessage = "Title cannot be longer than 32 characters")]
    string Name,
    [Required(ErrorMessage = "Patronymic is required")]
    [MaxLength(32, ErrorMessage = "Title cannot be longer than 32 characters")]
    string Patronymic,
    bool IsActive = true);