using Application.DTOs.Base;
using Application.DTOs.Department;
using Application.DTOs.Group;

namespace Application.DTOs.Metrics;

public record OrderHistogramRequest(
    DateRangeDto DateRange,
    bool? IsFullTime = null,
    IEnumerable<int>? Departments = null,
    IEnumerable<int>? Specializations = null,
    IEnumerable<GroupKeyDto>? Groups = null);