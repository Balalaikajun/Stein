using Application.DTOs.Department;
using Application.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Application.Services;

public class DepartmentService : IDepartmentService
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private static readonly Dictionary<string, Expression<Func<Department, object>>> SortSelectors = new()
    {
        ["Title"] = d => d.Title,
        ["IsActive"] = d => d.IsActive,
        ["Id"] = d => d.Id
    };

    public DepartmentService(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<DepartmentOptionResultDto> GetDepartments(DepartmentOptionRequestDto request)
    {
        var query = _context.Departments.AsQueryable();

        // Фильтрация
        if (request.ActiveFilter.HasValue)
            query = query.Where(d => d.IsActive == request.ActiveFilter.Value);

        if (!string.IsNullOrWhiteSpace(request.SearchText))
            query = query.Where(d => EF.Functions.Like(d.Title, $"%{request.SearchText}%"));

        // Сортировка
        var sortBy = SortSelectors.ContainsKey(request.SortBy) ? request.SortBy : "Title";
        var sortSelector = SortSelectors[sortBy];

        query = request.Descending
            ? query.OrderByDescending(sortSelector).ThenByDescending(d => d.Id)
            : query.OrderBy(sortSelector).ThenBy(d => d.Id);

        int? total = null;
        if (request.Skip == 0)
        {
            total = await query.CountAsync();
        }
        
        // Пагинацияs
        var items = await query
            .Skip(request.Skip)
            .Take(request.Take + 1) // берём на 1 больше, чтобы узнать, есть ли ещё
            .ProjectTo<DepartmentGetDto>(_mapper.ConfigurationProvider)
            .ToListAsync();

        
        
        var hasMore = items.Count > request.Take;
        var resultItems = hasMore ? items.Take(request.Take) : items;

        return new DepartmentOptionResultDto(
            Items: resultItems,
            HasMore: hasMore,
            Total: total);
    }
}
