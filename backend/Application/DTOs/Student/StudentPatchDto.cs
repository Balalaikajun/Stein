using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Application.DTOs.Student;

public record StudentPatchDto(
    [Required(ErrorMessage = "Id is required")]
    int Id,
    [MaxLength(32, ErrorMessage = "Surname cannot be longer than 32 characters")]
    string? Surname,
    [MaxLength(32, ErrorMessage = "Name cannot be longer than 32 characters")]
    string? Name,
    [MaxLength(32, ErrorMessage = "Patronymic cannot be longer than 32 characters")]
    string? Patronymic,
    Gender? Gender,
    DateOnly? DateOfBirth,
    bool? IsCitizen,
    StudentStatuses? StudentStatus);