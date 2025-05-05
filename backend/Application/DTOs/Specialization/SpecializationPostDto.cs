using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Specialization;

public record SpecializationPostDto(
    [Required(ErrorMessage = "Title is required")]
    [MaxLength(150, ErrorMessage = "Title cannot be longer than 150 characters")]
    string Title,
    [Required(ErrorMessage = "Code is required")]
    [MaxLength(25, ErrorMessage = "Title cannot be longer than 25 characters")]
    string Code,
    [Required(ErrorMessage = "Acronym is required")]
    [MaxLength(15, ErrorMessage = "Title cannot be longer than 15 characters")]
    string Acronym,
    [Required(ErrorMessage = "DepartmentId is required")]
    int DepartmentId,
    [Required(ErrorMessage = "IsActive is required")]
    bool IsActive);