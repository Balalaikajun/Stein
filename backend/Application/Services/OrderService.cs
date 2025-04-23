using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using Application.DTOs.Department;
using Application.DTOs.Order;
using Application.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class OrderService: IOrderService
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private static readonly Dictionary<string, Expression<Func<Order, object>>> _sortSelectors = new()
    {
        ["OrderType"] = o => o.OrderType.ToString(),
        ["OrderNumber"] = o => o.OrderNumber,
        ["Student"] = o => o.Student.Surname,
        ["Date"] = o => o.Date
    };

    public OrderService(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<BasePaginatedResult<OrderGetDto>> GetPaginated(OrderPaginatedRequest request)
    {
        var query = _context.Orders.AsQueryable();

        // Фильтрация
        if (request.OrderTypesFilter!=null && request.OrderTypesFilter.Any())
            query = query.Where(d => request.OrderTypesFilter.Contains(d.OrderType));

        if (!string.IsNullOrWhiteSpace(request.SearchText))
            query = query.Where(d => EF.Functions.Like(d.OrderNumber, $"%{request.SearchText}%"));

        if (request.DateRangeFilter!= null && request.DateRangeFilter.FromDate.HasValue)
            query = query.Where(s => s.Date >= request.DateRangeFilter.FromDate);

        if (request.DateRangeFilter != null && request.DateRangeFilter.ToDate.HasValue)
            query = query.Where(s => s.Date<= request.DateRangeFilter.ToDate);
        
        // Сортировка
        var sortBy = _sortSelectors.ContainsKey(request.SortBy) ? request.SortBy : "Date";
        var sortSelector = _sortSelectors[sortBy];

        query = request.Descending
            ? query.OrderByDescending(sortSelector).ThenByDescending(d => d.Id)
            : query.OrderBy(sortSelector).ThenBy(d => d.Id);

        int? total = null;
        if (request.Skip == 0)
        {
            total = await query.CountAsync();
        }
        
        // Пагинация
        var items = await query
            .Skip(request.Skip)
            .Take(request.Take + 1) 
            .ProjectTo<OrderGetDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
        
        var hasMore = items.Count > request.Take;
        var resultItems = hasMore ? items.Take(request.Take) : items;

        return new BasePaginatedResult<OrderGetDto>(
            Items: resultItems,
            HasMore: hasMore,
            Total: total);
    }
    
}