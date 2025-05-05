using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Teacher;

public record TeacherPatchDto(
    [Required(ErrorMessage = "Id is required")]
    int Id,
    string? Surname,
    string? Name,
    string? Patronymic,
    bool? IsActive = true);