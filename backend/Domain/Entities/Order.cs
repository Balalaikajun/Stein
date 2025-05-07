using Domain.Enums;

namespace Domain.Entities;

public abstract class Order
{
    public int Id { get; set; }
    public string Discriminator { get; set; }
    public OrderTypes OrderType { get;  set; }
    public string OrderNumber { get; set; }
    public int StudentId { get; set; }
    public DateOnly Date { get; set; }
    
    public Student Student { get; set; }
}