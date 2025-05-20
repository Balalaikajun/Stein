namespace Application.DTOs.Metrics;

public record ContingentHistogramDto(
    IEnumerable<ContingentDto> Data);
