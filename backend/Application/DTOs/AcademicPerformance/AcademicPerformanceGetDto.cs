using Domain.Enums;

namespace Application.DTOs.AcademicPerformance;

public class AcademicPerformanceGetDto
{
    public int Year { get; init; }
    public int Month { get; init; }
    public int StudentId { get; init; }
    public PerformanceCategory Performance { get; init; }
}