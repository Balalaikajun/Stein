using System.Collections.ObjectModel;

namespace Domain.Entities;

public class Teacher
{
    public int Id { get; set; }
    public string Surname { get; set; }
    public string Name { get; set; }
    public string Patronymic { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation
    public ICollection<Group> Groups { get; }
        = new Collection<Group>();
}