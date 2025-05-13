namespace Application.DTOs.Metrics;

public record PieDto(
    IEnumerable<PieSegment> Segments
    );