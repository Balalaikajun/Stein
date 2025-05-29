using Domain.Enums;

namespace Domain.Entities;

public class AcademicPerformance
{
    public int Year { get; set; }
    public int Month { get; set; }
    public int StudentId { get; set; }
    public PerformanceCategory Performance { get; set; }

    // Navigation
    public Student Student { get; set; }
}