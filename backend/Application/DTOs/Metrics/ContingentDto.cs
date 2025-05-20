namespace Application.DTOs.Metrics;

public record ContingentDto(
    string Department,
    string Specialization,
    string Course,
    string Group,
    int Count);