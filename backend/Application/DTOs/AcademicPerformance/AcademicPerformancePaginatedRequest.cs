using Application.DTOs.Group;

namespace Application.DTOs.AcademicPerformance;

public record AcademicPerformancePaginatedRequest(
    int Skip,
    int Take,
    IEnumerable<GroupKeyDto> GroupKeys,
    bool Descending = false);