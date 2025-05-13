namespace Application.DTOs.Metrics;

public record CountsDto(
    CountDto Students,
    CountDto Orders,
    CountDto Foreign
);