using Domain.Enums;

namespace Application.DTOs.Student;

public record StudentPaginatedRequest(
    int Skip = 0,
    int Take = 10,
    string? SearchText = null,
    bool? ActiveFilter = null,
    IEnumerable<int>? DepartmentIds = null,
    IEnumerable<int>? SpecializationIds = null,
    IEnumerable<int>? Years = null,
    bool? IsCitizen = null,
    Gender? Gender = null,
    StudentStatuses? Status = null,
    DateOnly? BirthDateFrom = null,
    DateOnly? BirthDateTo = null,
    string SortBy = "Title",
    bool Descending = false);