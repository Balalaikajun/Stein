namespace Application.DTOs.Specialization;

public record SpecializationPaginatedRequest(
    int Skip = 0,
    int Take = 10,
    string? SearchText = null,
    bool? ActiveFilter = null,
    IEnumerable<int>? DepartmentIds = null,
    string SortBy = "Title",
    bool Descending = false);