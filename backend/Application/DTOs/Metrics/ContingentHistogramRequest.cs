namespace Application.DTOs.Metrics;

public record ContingentHistogramRequest(
    DateOnly Date,
    bool? IsFullTime = null);