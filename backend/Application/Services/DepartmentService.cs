using System.Linq.Expressions;
using Application.DTOs.Base;
using Application.DTOs.Department;
using Application.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class DepartmentService : IDepartmentService
{
    private static readonly Dictionary<string, Expression<Func<Department, object>>> _sortSelectors = new()
    {
        ["Title"] = d => d.Title,
        ["IsActive"] = d => d.IsActive,
        ["Id"] = d => d.Id
    };

    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public DepartmentService(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<BasePaginatedResult<DepartmentGetDto>> GetPaginated(DepartmentPaginatedRequest request)
    {
        var query = _context.Departments.AsQueryable();

        // Фильтрация
        if (request.ActiveFilter.HasValue)
            query = query.Where(d => d.IsActive == request.ActiveFilter.Value);

        if (!string.IsNullOrWhiteSpace(request.SearchText))
            query = query.Where(d => EF.Functions.Like(d.Title, $"%{request.SearchText}%"));

        // Сортировка
        var sortBy = _sortSelectors.ContainsKey(request.SortBy) ? request.SortBy : "Title";
        var sortSelector = _sortSelectors[sortBy];

        query = request.Descending
            ? query.OrderByDescending(sortSelector).ThenByDescending(d => d.Id)
            : query.OrderBy(sortSelector).ThenBy(d => d.Id);

        int? total = null;
        if (request.Skip == 0) total = await query.CountAsync();

        // Пагинацияs
        var items = await query
            .Skip(request.Skip)
            .Take(request.Take + 1) // берём на 1 больше, чтобы узнать, есть ли ещё
            .ProjectTo<DepartmentGetDto>(_mapper.ConfigurationProvider)
            .ToListAsync();


        var hasMore = items.Count > request.Take;
        var resultItems = hasMore ? items.Take(request.Take) : items;

        return new BasePaginatedResult<DepartmentGetDto>(
            resultItems,
            hasMore,
            total);
    }

    public async Task Create(DepartmentPostDto dto)
    {
        var department = _mapper.Map<Department>(dto);

        _context.Departments.Add(department);

        await _context.SaveChangesAsync();
    }


    public async Task Update(DepartmentPatchDto dto)
    {
        var department = await _context.Departments.FirstOrDefaultAsync(x => x.Id == dto.Id) ??
                         throw new NotFoundException($"Department with id - {dto.Id} was not found");

        if (!string.IsNullOrWhiteSpace(dto.Title) && department.Title != dto.Title)
            department.Title = dto.Title;

        if (dto.IsActive.HasValue && dto.IsActive != department.IsActive)
            department.IsActive = dto.IsActive.Value;

        await _context.SaveChangesAsync();
    }

    public async Task Delete(int id)
    {
        var department = await _context.Departments.FirstOrDefaultAsync(x => x.Id == id);

        if (department == null)
            throw new NotFoundException($"Department with id - {id} was not found");

        _context.Departments.Remove(department);

        await _context.SaveChangesAsync();
    }
}