using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using Application.DTOs.Department;
using Application.DTOs.Specialization;
using Application.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;
using Domain.Exceptions;
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

    public async Task<BasePaginatedResult<SpecializationGetDto>> GetPaginated(SpecializationPaginatedRequest request)
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

        return new BasePaginatedResult<SpecializationGetDto>(
            Items: resultItems,
            HasMore: hasMore,
            Total: total);
    }
    
    public async Task Create(SpecializationPostDto dto)
    {
        if(!await _context.Departments.AnyAsync(x => x.Id == dto.DepartmentId))
            throw new NotFoundException($"Department with id - {dto.DepartmentId} was not found");
        
        var specialization = _mapper.Map<Specialization>(dto);
        
        _context.Specializations.Add(specialization);

        await _context.SaveChangesAsync();
    }
    
    
    public async Task Update(SpecializationPatchDto dto)
    {
        var specialization = await _context.Specializations.FirstOrDefaultAsync(x => x.Id == dto.Id) ??
                      throw new NotFoundException($"Specialization with id - {dto.Id} was not found");
        
        if (!string.IsNullOrWhiteSpace(dto.Title) && specialization.Title != dto.Title)
            specialization.Title = dto.Title;
        
        if (!string.IsNullOrWhiteSpace(dto.Code) && specialization.Code != dto.Code)
            specialization.Code = dto.Code;
        
        if (!string.IsNullOrWhiteSpace(dto.Acronym) && specialization.Acronym != dto.Acronym)
            specialization.Acronym = dto.Acronym;

        if (dto.DepartmentId.HasValue && dto.DepartmentId != specialization.DepartmentId)
        {
            if(!await _context.Departments.AnyAsync(x => x.Id == dto.DepartmentId))
                throw new NotFoundException($"Department with id - {dto.DepartmentId} was not found");
            
            specialization.DepartmentId = dto.DepartmentId.Value;
        }
        
        if(dto.IsActive.HasValue && dto.IsActive != specialization.IsActive)
            specialization.IsActive = dto.IsActive.Value;

        await _context.SaveChangesAsync();
    }

    public async Task Delete(int id)
    {
        var specialization = await _context.Specializations.FirstOrDefaultAsync(x => x.Id == id) ??
                      throw new NotFoundException($"Specialization with id - {id} was not found");
        
        _context.Specializations.Remove(specialization);
        
        await _context.SaveChangesAsync();
    }
}