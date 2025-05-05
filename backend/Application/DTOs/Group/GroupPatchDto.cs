using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Group;

public record GroupPatchDto(
    GroupKeyDto Key,
    [Required(ErrorMessage = "SpecializationId is required")]
    int? SpecializationId,
    [Required(ErrorMessage = "Year is required")]
    int? Year,
    [MaxLength(3, ErrorMessage = "Id cannot be more than 3 characters")]
    string? Id,
    [MaxLength(10, ErrorMessage = "Acronym cannot be more than 10 characters")]
    string? Acronym,
    bool? IsActive,
    int? TeacherId);