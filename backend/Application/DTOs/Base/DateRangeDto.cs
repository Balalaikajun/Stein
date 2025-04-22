namespace Application.DTOs.Department;

public record DateRangeDto(
    DateOnly? FromDate = null,
    DateOnly? ToDate = null);