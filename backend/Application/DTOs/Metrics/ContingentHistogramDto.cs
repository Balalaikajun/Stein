namespace Application.DTOs.Metrics;

public record ContingentHistogramDto(
    int Count,
    IEnumerable<ContingentDto> Data);