using Application.DTOs.Group;
using Domain.Enums;

namespace Application.DTOs.Order;

public record OrderPostDto(
    OrderTypes Type,
    string Number,
    GroupKeyDto? GroupFrom,
    GroupKeyDto? GroupTo,
    int StudentId,
    DateOnly Date
);