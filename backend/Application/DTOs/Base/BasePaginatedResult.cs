using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Department;

public record BasePaginatedResult<TDto>(
    IEnumerable<TDto> Items,
    bool HasMore,
    int? Total = null);