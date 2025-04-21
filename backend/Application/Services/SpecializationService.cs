using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using Application.DTOs.Department;
using Application.DTOs.Specialization;
using Application.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class SpecializationService:ISpecializationService
{
    private IApplicationDbContext _context;
    private IMapper _mapper;
    private static readonly Dictionary<string, Expression<Func<Specialization, object>>> _sortSelectors = new()
    {
        ["Id"] = s => s.Id,
        ["Code"] = s => s.Code,
        ["Title"] = s => s.Acronym,
        ["Acronym"] = s => s.Acronym,
        ["IsActive"] = s => s.IsActive,
        ["Department"] = s => s.Department.Title,
    };

    public SpecializationService(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<BasePaginatedResultDto<SpecializationGetDto>> GetPaginated(SpecializationPaginatedRequestDto request)
    {
        var query = _context.Specializations.AsQueryable();

        // Фильтрация
        if (request.ActiveFilter.HasValue)
            query = query.Where(d => d.IsActive == request.ActiveFilter.Value);

        if ( request.DepartmentIds != null && request.DepartmentIds.Any())
        {
            query = query.Where(d => request.DepartmentIds.Contains(d.DepartmentId));
        }
        
        if (!string.IsNullOrWhiteSpace(request.SearchText))
            query = query.Where(d => EF.Functions.Like(d.Title, $"%{request.SearchText}%"));

        // Сортировка
        var sortBy = _sortSelectors.ContainsKey(request.SortBy) ? request.SortBy : "Title";
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
            .Include(s => s.Department)
            .Skip(request.Skip)
            .Take(request.Take + 1) // берём на 1 больше, чтобы узнать, есть ли ещё
            .ProjectTo<SpecializationGetDto>(_mapper.ConfigurationProvider)
            .ToListAsync();

        
        
        var hasMore = items.Count > request.Take;
        var resultItems = hasMore ? items.Take(request.Take) : items;

        return new BasePaginatedResultDto<SpecializationGetDto>(
            Items: resultItems,
            HasMore: hasMore,
            Total: total);
    }
}