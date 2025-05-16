using Application.DTOs.Group;

namespace Application.DTOs.Metrics;

public record PerformanceRequest(
    int YearFrom, 
    int MonthFrom,
    int YearTo,
    int MonthTo,
    bool? IsFullTime = null,
    IEnumerable<int>? Departments = null,
    IEnumerable<int>? Specializations = null,
    IEnumerable<GroupKeyDto>? Groups = null
    );