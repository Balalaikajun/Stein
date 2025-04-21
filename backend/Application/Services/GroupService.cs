using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using Application.DTOs.Department;
using Application.DTOs.Group;
using Application.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class GroupService: IGroupService
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private static readonly Dictionary<string, Expression<Func<Group, object>>> _sortSelectors = new()
    {
        ["Acronym"] = g => g.Acronym,
        ["IsActive"] = g => g.IsActive,
        ["Year"] = g => g.Year,
        ["Specialization"] = g => g.Specialization.Title,
        ["Department"] = g => g.Specialization.Department.Title,
        ["Teacher"] = g => g.Teacher.Surname,
        ["Students"] = g => g.Students.Count
    };

    public GroupService(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<BasePaginatedResult<GroupGetDto>> GetPaginated(GroupPaginatedRequest request)
    {
        var query = _context.Groups.AsQueryable();

        // Фильтрация
        if (request.ActiveFilter.HasValue)
            query = query.Where(d => d.IsActive == request.ActiveFilter.Value);

        if (!string.IsNullOrWhiteSpace(request.SearchText))
            query = query.Where(d => EF.Functions.Like(d.Acronym, $"%{request.SearchText}%"));

        query = query
            .Join(_context.Specializations,
                g => g.SpecializationId,
                s => s.Id,
                (g, s) => new { Group = g, Specialization = s })
            .Where(x => (request.DepartmentIds == null || !request.DepartmentIds.Any() ||
                         request.DepartmentIds.Contains(x.Specialization.DepartmentId)) &&
                        (request.SpecializationIds == null || !request.SpecializationIds.Any() ||
                         request.SpecializationIds.Contains(x.Specialization.Id)))
            .Select(x => x.Group);

        if (request.Years != null && request.Years.Any())
        {
            query = query.Where(g => request.Years.Contains(g.Year));
        }

        // Сортировка
        var sortBy = _sortSelectors.ContainsKey(request.SortBy) ? request.SortBy : "Acronym";
        var sortSelector = _sortSelectors[sortBy];

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
            .ProjectTo<GroupGetDto>(_mapper.ConfigurationProvider)
            .ToListAsync();

        
        
        var hasMore = items.Count > request.Take;
        var resultItems = hasMore ? items.Take(request.Take) : items;

        return new BasePaginatedResult<GroupGetDto>(
            Items: resultItems,
            HasMore: hasMore,
            Total: total);
    }
    
}