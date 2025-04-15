using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Department;

public record DepartmentOptionRequestDto(
    string? LastSeenValue,
    int? LastSeenId,
    string? SearchText,
    bool? ActiveFilter,
    string SortBy = "Title",
    bool Descending = false,
    int Limit = 10);