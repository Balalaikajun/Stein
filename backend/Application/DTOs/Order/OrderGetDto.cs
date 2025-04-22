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
    public string? GroupFromAcronym { get; init; }
    public string? GroupToAcronym { get; init; }
}