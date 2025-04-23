using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using Application.DTOs.Department;
using Application.DTOs.Group;
using Application.DTOs.Student;
using Application.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;
using Domain.Interfaces;
using LinqKit;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class StudentService: IStudentService
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private static readonly Dictionary<string, Expression<Func<Student, object>>> _sortSelectors = new()
    {
        ["Surname"] = s => s.Surname,
        ["Name"] = s => s.Name,
        ["Patronymic"] = s => s.Patronymic,
        ["Group"] = s => s.Group.Acronym,
        ["IsCitizen"] = s => s.IsCitizen,
        ["Gender"] = s => s.Gender,
        ["DateOfBirth"] = s => s.DateOfBirth,
        ["Status"] = s => s.Status,
    };

    public StudentService(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

  public async Task<BasePaginatedResult<StudentGetDto>> GetPaginated(StudentPaginatedRequest request)
{
    var query = _context.Students
        .Include(s => s.Group)
        .ThenInclude(g => g.Specialization)
        .AsQueryable();

    // Фильтр по статусу
    if (request.StudentStatusesFilter?.Any()==true)
        query = query.Where(s => request.StudentStatusesFilter.Contains(s.Status));

    // Фильтр по отделениям и специализациям
    if (request.DepartmentIds?.Any() == true)
        query = query.Where(s => request.DepartmentIds.Contains(s.Group.Specialization.DepartmentId));

    if (request.SpecializationIds?.Any() == true)
        query = query.Where(s => request.SpecializationIds.Contains(s.Group.SpecializationId));

    if (request.GroupFilter?.Any() == true)
    {
        var groupPredicate = PredicateBuilder.New<Student>(false);
        foreach (var k in request.GroupFilter)
        {
            groupPredicate = groupPredicate.Or(s => 
                s.GroupSpecializationId == k.SpecializationId &&
                s.GroupYear== k.Year &&
                s.GroupId == k.Id);
        }
        query = query.Where(groupPredicate);
    }

    // Фильтр по текстовому поиску
    if (!string.IsNullOrWhiteSpace(request.SearchText))
    {
        query = query.Where(s =>
            EF.Functions.Like(s.Surname, $"%{request.SearchText}%") ||
            EF.Functions.Like(s.Name, $"%{request.SearchText}%") ||
            EF.Functions.Like(s.Patronymic, $"%{request.SearchText}%")
        );
    }

    // Фильтр по годам обучения
    if (request.Years?.Any() == true)
        query = query.Where(s => request.Years.Contains(s.Group.Year));

    // Фильтр по гражданству
    if (request.IsCitizen.HasValue)
        query = query.Where(s => s.IsCitizen == request.IsCitizen);

    // Фильтр по полу
    if (request.Gender.HasValue)
        query = query.Where(s => s.Gender == request.Gender.Value);

    // Фильтр по дате рождения
    if (request.DateRange!= null && request.DateRange.FromDate.HasValue)
        query = query.Where(s => s.DateOfBirth >= request.DateRange.FromDate);

    if (request.DateRange != null && request.DateRange.ToDate.HasValue)
        query = query.Where(s => s.DateOfBirth <= request.DateRange.ToDate);

    // Сортировка
    var sortBy = _sortSelectors.ContainsKey(request.SortBy) ? request.SortBy : "Surname";
    var sortSelector = _sortSelectors[sortBy];

    query = request.Descending
        ? query.OrderByDescending(sortSelector).ThenByDescending(s => s.Id)
        : query.OrderBy(sortSelector).ThenBy(s => s.Id);

    // Общее количество
    int? total = null;
    if (request.Skip == 0)
        total = await query.CountAsync();

    // Пагинация
    var items = await query
        .Skip(request.Skip)
        .Take(request.Take + 1) // +1 чтобы определить, есть ли ещё
        .ProjectTo<StudentGetDto>(_mapper.ConfigurationProvider)
        .ToListAsync();

    var hasMore = items.Count > request.Take;
    var resultItems = hasMore ? items.Take(request.Take) : items;

    return new BasePaginatedResult<StudentGetDto>(
        Items: resultItems,
        HasMore: hasMore,
        Total: total
    );
}

    
    
}