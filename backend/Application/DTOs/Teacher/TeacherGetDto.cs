using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Teacher;

public record TeacherGetDto(
    int Id,
    string Surname,
    string Name,
    string Patronymic,
    bool IsActive);
    