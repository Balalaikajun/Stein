namespace Domain.Entities;

public class User
{
    public int Id { get; private set;}
    public string Login { get; set; }
    public string HashedPassword { get; set; }
}