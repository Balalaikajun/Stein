namespace Application.DTOs.Base;

public record DateRangeDto(
    DateOnly? FromDate = null,
    DateOnly? ToDate = null);