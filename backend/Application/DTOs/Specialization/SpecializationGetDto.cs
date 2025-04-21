namespace Application.DTOs.Specialization;

public record SpecializationGetDto(
    int Id,
    string Title,
    string Code,
    string Acronym,
    string DepartmentTitle,
    bool IsActive);