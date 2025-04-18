using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Department;

public record DepartmentOptionResultDto(
    IEnumerable<DepartmentGetDto> Items,
    bool HasMore);