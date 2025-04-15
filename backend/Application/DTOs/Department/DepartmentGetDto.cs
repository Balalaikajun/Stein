using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Department;

public record DepartmentGetDto(
    [Required(ErrorMessage = "Id is required")]
    int Id,
    [Required(ErrorMessage = "Title is required")]
    string Title,
    bool IsActive);