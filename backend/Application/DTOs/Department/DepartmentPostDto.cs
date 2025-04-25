using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Department;

public record DepartmentPostDto(
    [Required(ErrorMessage = "Name is required")]
    [MaxLength(125, ErrorMessage = "Title cannot be longer than 125 characters")]
    string Title,
    bool IsActive = true);