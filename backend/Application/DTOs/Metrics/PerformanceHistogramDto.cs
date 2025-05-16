namespace Application.DTOs.Metrics;

public record PerformanceHistogramDto(
    IEnumerable<PerformanceElementDto> Elements,
    bool HasBefore,
    bool HasAfter);