using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Department;

public record DepartmentPatchDto(
    [Required(ErrorMessage="Id is required")]
    int Id,
    [MaxLength(125, ErrorMessage = "Title cannot be longer than 125 characters")]
    string? Title,
    bool? IsActive = true);