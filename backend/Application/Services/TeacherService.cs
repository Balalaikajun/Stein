using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using Application.DTOs.Department;
using Application.DTOs.Teacher;
using Application.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class TeacherService: ITeacherService
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private static readonly Dictionary<string, Expression<Func<Teacher, object>>> _sortSelectors = new()
    {
        ["Id"] = t => t.Id,
        ["Surname"] = t => t.Surname,
        ["Name"] = t => t.Name,
        ["Patronymic"] = t => t.Patronymic,
        ["IsActive"] = t => t.IsActive
    };

    public TeacherService(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<BasePaginatedResult<TeacherGetDto>> GetPaginated(TeacherPaginatedRequest request)
    {
        var query = _context.Teachers.AsQueryable();

        // Фильтрация
        if (request.ActiveFilter.HasValue)
            query = query.Where(d => d.IsActive == request.ActiveFilter.Value);

        if (!string.IsNullOrWhiteSpace(request.SearchText))
            query = query.Where(d =>
                EF.Functions.Like(d.Surname, $"%{request.SearchText}%") ||
                EF.Functions.Like(d.Name, $"%{request.SearchText}%") ||
                EF.Functions.Like(d.Patronymic, $"%{request.SearchText}%")
            );

        // Сортировка
        var sortBy = _sortSelectors.ContainsKey(request.SortBy) ? request.SortBy : "Surname";
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
            .ProjectTo<TeacherGetDto>(_mapper.ConfigurationProvider)
            .ToListAsync();

        
        
        var hasMore = items.Count > request.Take;
        var resultItems = hasMore ? items.Take(request.Take) : items;

        return new BasePaginatedResult<TeacherGetDto>(
            Items: resultItems,
            HasMore: hasMore,
            Total: total);
    }
    
    public async Task Create(TeacherPostDto dto)
    {
        var teacher = _mapper.Map<Teacher>(dto);
        
        _context.Teachers.Add(teacher);

        await _context.SaveChangesAsync();
    }
    
    
    public async Task Update(TeacherPatchDto dto)
    {
        var teacher = await _context.Teachers.FirstOrDefaultAsync(x => x.Id == dto.Id) ??
                         throw new NotFoundException($"Teacher with id - {dto.Id} was not found");
        
        if (!string.IsNullOrWhiteSpace(dto.Surname) && teacher.Surname != dto.Surname)
            teacher.Surname = dto.Surname;
        
        if (!string.IsNullOrWhiteSpace(dto.Name) && teacher.Name != dto.Name)
            teacher.Name = dto.Name;
        
        if (!string.IsNullOrWhiteSpace(dto.Patronymic) && teacher.Patronymic != dto.Patronymic)
            teacher.Patronymic = dto.Patronymic;
        
        if(dto.IsActive.HasValue && dto.IsActive != teacher.IsActive)
            teacher.IsActive = dto.IsActive.Value;

        await _context.SaveChangesAsync();
    }

    public async Task Delete(int id)
    {
        var teacher = await _context.Teachers.FirstOrDefaultAsync(x => x.Id == id) ??
                      throw new NotFoundException($"Teacher with id - {id} was not found");
        
        _context.Teachers.Remove(teacher);
        
        await _context.SaveChangesAsync();
    }
}