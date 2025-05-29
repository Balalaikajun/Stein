namespace Application.DTOs.Base;

public record BasePaginatedResult<TDto>(
    IEnumerable<TDto> Items,
    bool HasMore,
    int? Total = null);