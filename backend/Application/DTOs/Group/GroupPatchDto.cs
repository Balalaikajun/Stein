using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Group;

public record GroupPatchDto(
    GroupKeyDto Id,
    int? NewSpecializationId,
    int? NewYear,
    [MaxLength(3, ErrorMessage = "Id cannot be more than 3 characters")]
    string? NewIndex,
    [MaxLength(10, ErrorMessage = "Acronym cannot be more than 10 characters")]
    string? NewAcronym,
    bool? NewIsActive,
    int? NewTeacherId);