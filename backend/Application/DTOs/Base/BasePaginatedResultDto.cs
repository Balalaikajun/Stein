using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Department;

public record BasePaginatedResultDto<TDto>(
    IEnumerable<TDto> Items,
    bool HasMore,
    int? Total = null);