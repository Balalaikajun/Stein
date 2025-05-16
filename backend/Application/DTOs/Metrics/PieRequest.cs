using Application.DTOs.Group;

namespace Application.DTOs.Metrics;

public record PieRequest(
    DateOnly? Date = null,
    bool? IsFullTime = null,
    IEnumerable<int>? Departments = null,
    IEnumerable<int>? Specializations = null,
    IEnumerable<GroupKeyDto>? Groups = null);