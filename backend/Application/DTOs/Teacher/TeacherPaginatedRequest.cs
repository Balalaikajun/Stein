namespace Application.DTOs.Teacher;

public record TeacherPaginatedRequest(
    int Skip = 0,
    int Take = 10,
    string? SearchText = null,
    bool? ActiveFilter = null,
    string SortBy = "Title",
    bool Descending = false);