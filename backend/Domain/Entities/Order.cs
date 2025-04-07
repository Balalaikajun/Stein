using Domain.Enums;

namespace Domain.Entities;

public abstract class Order
{
    public int Id { get; private set; }
    public OrderTypes OrderType { get; protected set; }
    public string OrderNumber { get; set; }
    public int StudentID { get; set; }
    public DateOnly Date { get; set; }
    
    public Student Student { get; set; }
}