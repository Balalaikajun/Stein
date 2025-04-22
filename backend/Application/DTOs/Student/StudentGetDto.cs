using Domain.Enums;

namespace Application.DTOs.Student;

public record StudentGetDto(
    int Id,
    string Surname,
    string Name,
    string Patronymic,
    string GroupAcronym,
    bool IsCitizen,
    Gender Gender,
    DateOnly DateOfBirth,
    StudentStatuses Status
);