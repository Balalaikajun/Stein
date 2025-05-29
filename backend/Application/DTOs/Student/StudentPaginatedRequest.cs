using Application.DTOs.Base;
using Application.DTOs.Department;
using Application.DTOs.Group;
using Domain.Enums;

namespace Application.DTOs.Student;

public record StudentPaginatedRequest(
    int Skip = 0,
    int Take = 10,
    string? SearchText = null,
    IEnumerable<StudentStatuses>? StudentStatusesFilter = null,
    IEnumerable<int>? DepartmentIds = null,
    IEnumerable<int>? SpecializationIds = null,
    IEnumerable<int>? Years = null,
    IEnumerable<GroupKeyDto>? GroupKeys = null,
    bool? IsCitizen = null,
    Gender? Gender = null,
    DateRangeDto? DateRange = null,
    string SortBy = "Title",
    bool Descending = false);