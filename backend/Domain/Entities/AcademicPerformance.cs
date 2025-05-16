using System.Diagnostics.Contracts;
using Domain.Enums;

namespace Domain.Entities;

public class AcademicPerformance
{
    public int Year { get; private set; }
    public int Month { get; private set; }
    public int StudentId { get; private set; }
    public PerformanceCategory Performance { get; set; }
    
    // Navigation
    public Student Student { get; set; }
} 