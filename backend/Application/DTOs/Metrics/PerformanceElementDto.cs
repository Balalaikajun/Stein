using Domain.Enums;

namespace Application.DTOs.Metrics;

public record PerformanceElementDto(
    int Year,
    int Month,
    int Count,
    Dictionary<PerformanceCategory, int> Data);