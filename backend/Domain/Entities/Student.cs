using Domain.Enums;

namespace Domain.Entities;

public class Student
{
    public int Id { get; private set; }
    public string Surname { get; set; }
    public string Name { get; set; }
    public string Patronymic { get; set; }
    public int GroupSpecializationId { get; set; }
    public int GroupYear { get; set; }
    public string GroupId { get; set; }
    public bool IsCitizen { get; set; } 
    public Gender Gender { get; set; }
    public DateOnly DateOfBirth { get; set; }
    public StudentStatuses Status { get; set; } = StudentStatuses.Active;

    // Navigation
    public Group? Group { get; }
    public ICollection<AcademicPerformance> AcademicPerformance { get; private set;} 
        = new  List<AcademicPerformance>();
}