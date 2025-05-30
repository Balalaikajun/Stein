namespace Domain.Entities;

public class Group
{
    public int SpecializationId { get; set; }
    public int Year { get; set; }
    public string Index { get; set; }
    public string Acronym { get; set; }
    public int TeacherId { get; set; }
    public bool IsActive { get; set; } = true;
    public DateOnly? ReleaseDate { get; set; }

    // Navigation
    public Specialization? Specialization { get; set; }
    public Teacher? Teacher { get; set; }
    public ICollection<Student>? Students { get; set; }
    public ICollection<Order>? EnrollmentOrders { get; set; }

    public ICollection<Order>? ExplitOrders { get; set; }
}