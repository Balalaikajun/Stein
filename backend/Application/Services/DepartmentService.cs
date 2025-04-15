using System.Linq.Dynamic.Core;
using Application.DTOs.Department;
using Application.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class DepartmentService : IDepartmentService
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public DepartmentService(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<DepartmentOptionResultDto> GetDepartments(DepartmentOptionRequestDto request)
{
    var query = _context.Departments.AsQueryable();

    // Применяем фильтры
    if (request.ActiveFilter.HasValue)
        query = query.Where(d => d.IsActive == request.ActiveFilter.Value);

    if (!string.IsNullOrWhiteSpace(request.SearchText))
        query = query.Where(d => EF.Functions.Like(d.Title, $"%{request.SearchText}%"));

    // Определяем направление сортировки
    var sortProp = GetValidSortProperty(request.SortBy);
    var sortDirection = request.Descending ? "DESC" : "ASC";
    query = query.OrderBy($"{sortProp} {sortDirection}, Id {sortDirection}");

    // Обрабатываем курсорную пагинацию
    if (request.LastSeenId.HasValue && !string.IsNullOrEmpty(request.LastSeenValue))
    {
        var propType = GetPropertyType(sortProp);
        var value = ConvertValue(request.LastSeenValue, propType);

        string condition;
        if (propType == typeof(bool))
        {
            var boolValue = (bool)value;
            condition = request.Descending 
                ? boolValue 
                    ? $"({sortProp} == true && Id < @1) || {sortProp} == false" 
                    : "false" // Нет записей после false при сортировке DESC
                : boolValue 
                    ? $"({sortProp} == true && Id > @1)" 
                    : $"({sortProp} == false && Id > @1) || {sortProp} == true";
        }
        else
        {
            condition = request.Descending 
                ? $"{sortProp} < @0 || ({sortProp} == @0 && Id < @1)"
                : $"{sortProp} > @0 || ({sortProp} == @0 && Id > @1)";
        }

        query = query.Where(condition, value, request.LastSeenId.Value);
    }

    // Получаем данные
    var items = await query
        .Take(request.Limit + 1)
        .ProjectTo<DepartmentGetDto>(_mapper.ConfigurationProvider)
        .ToListAsync();

    var hasMore = items.Count > request.Limit;
    var resultItems = hasMore ? items.Take(request.Limit) : items;

    return new DepartmentOptionResultDto(
        resultItems.ToList(),
        resultItems.LastOrDefault()?.GetValue(sortProp)?.ToString(),
        resultItems.LastOrDefault()?.Id,
        hasMore);
}

private string GetValidSortProperty(string? sortBy)
{
    var validProps = new[] { "Title", "IsActive" };
    return validProps.Contains(sortBy, StringComparer.OrdinalIgnoreCase) 
        ? sortBy! 
        : "Title";
}

private Type GetPropertyType(string propertyName)
{
    return typeof(Department).GetProperty(propertyName)?.PropertyType 
        ?? typeof(string);
}

private object ConvertValue(string value, Type targetType)
{
    if (targetType == typeof(bool)) 
        return bool.Parse(value.ToLower());
    
    if (targetType == typeof(int)) 
        return int.Parse(value);
    
    return value;
}
}

public static class ObjectExtensions
{
    public static object? GetValue(this object obj, string propertyName)
    {
        return obj.GetType().GetProperty(propertyName)?.GetValue(obj);
    }
}