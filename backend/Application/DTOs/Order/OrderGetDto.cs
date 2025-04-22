using Application.DTOs.Group;
using Domain.Enums;

namespace Application.DTOs.Order;

public record OrderGetDto
{

    public int Id { get; init; }
    public OrderTypes OrderType { get; init; }
    public string OrderNumber { get; init; } = default!;
    public string StudentFullName { get; init; } = default!;
    public DateOnly Date { get; init; }
    public GroupKeyDto? GroupFrom { get; init; }
    public GroupKeyDto? GroupTo { get; init; }
}