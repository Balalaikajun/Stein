using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Specialization;

public record SpecializationPatchDto(
    [Required(ErrorMessage = "Id is required")]
    int Id,
    string? Title,
    string? Code,
    string? Acronym,
    int? DepartmentId,
    bool? IsActive);