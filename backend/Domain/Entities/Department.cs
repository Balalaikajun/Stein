namespace Domain.Entities;

public class Department
{
    public int Id { get; private set; }
    public string Title { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation
    public ICollection<Specialization> Specializations { get; set; }
        = new List<Specialization>();
}