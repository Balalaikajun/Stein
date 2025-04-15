using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Department;

public record DepartmentOptionResultDto(
    IEnumerable<DepartmentGetDto> Items,
    string? LastSeenValue,
    int? LastSeenId,
    bool HasMore);