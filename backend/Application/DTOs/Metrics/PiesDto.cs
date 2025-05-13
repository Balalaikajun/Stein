namespace Application.DTOs.Metrics;

public record PiesDto(
    PieDto Gender,
    PieDto Age,
    PieDto Citizenship);