using Domain.Enums;

namespace Domain.Entities;

public class Order
{
    public int Id { get; set; }
    public OrderTypes OrderType { get;  set; }
    public string Number { get; set; }
    public int StudentId { get; set; }
    public int? FromSpecializationId { get; set; }
    public int? FromYear { get; set; }
    public string? FromGroupId { get; set; }
    public int? ToSpecializationId { get; set; }
    public int? ToYear { get; set; }
    public string? ToGroupId { get; set; }
    public DateOnly Date { get; set; }
    
    public Student? Student { get; set; }
    public Group? FromGroup { get; set; }
    public Group? ToGroup { get; set; }
}