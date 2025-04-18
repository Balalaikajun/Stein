using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Department;

public record DepartmentOptionRequestDto(
    int Skip = 0,
    int Take = 10,
    string? SearchText = null,
    bool? ActiveFilter = null,
    string SortBy = "Title",
    bool Descending = false);
