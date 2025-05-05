using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Group;

public record GroupPostDto(
    [Required(ErrorMessage = "SpecializationId is required")]
    int SpecializationId,
    [Required(ErrorMessage = "Year is required")]
    int Year,
    [Required(ErrorMessage = "Id is required")]
    [MaxLength(3, ErrorMessage = "Id cannot be more than 3 characters")]
    string Id,
    [Required(ErrorMessage = "Acronym is required")]
    [MaxLength(10, ErrorMessage = "Acronym cannot be more than 10 characters")]
    string Acronym,
    [Required(ErrorMessage = "IsActive is required")]
    bool IsActive,
    int? TeacherId);