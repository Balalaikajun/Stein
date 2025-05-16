using Domain.Enums;

namespace Application.DTOs.Metrics;

public record OrderHistogramDto(
    int Count,
    Dictionary<OrderTypes, int> Data);