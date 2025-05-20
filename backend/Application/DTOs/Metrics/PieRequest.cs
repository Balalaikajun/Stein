using Application.DTOs.Group;

namespace Application.DTOs.Metrics;

public record PieRequest(
    bool? IsFullTime = null,
    IEnumerable<int>? DepartmentIds = null,
    IEnumerable<int>? SpecializationIds = null,
    IEnumerable<GroupKeyDto>? GroupKeys = null);