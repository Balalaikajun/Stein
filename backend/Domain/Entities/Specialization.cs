namespace Domain.Entities;

public class Specialization
{
    public int Id { get; set; }
    public string Code { get; set; }
    public string Title { get; set; }
    public string Acronym { get; set; }
    public int DepartmentId { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation
    public Department Department { get; set; }
}