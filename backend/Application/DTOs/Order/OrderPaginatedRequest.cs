using Application.DTOs.Department;
using Domain.Enums;

namespace Application.DTOs.Order;

public record OrderPaginatedRequest(
    int Skip = 0,
    int Take = 10,
    IEnumerable<OrderTypes>? OrderTypesFilter = null,
    DateRangeDto? DateRangeFilter = null,
    string? SearchText = null,
    string SortBy = "Title",
    bool Descending = false);