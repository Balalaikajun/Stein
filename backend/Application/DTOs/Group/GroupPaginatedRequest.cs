namespace Application.DTOs.Group;

public record GroupPaginatedRequest(
    int Skip = 0,
    int Take = 10,
    string? SearchText = null,
    bool? ActiveFilter = null,
    IEnumerable<int>? DepartmentIds = null,
    IEnumerable<int>? SpecializationIds = null,
    IEnumerable<int>? Years = null,
    string SortBy = "Title",
    bool Descending = false);