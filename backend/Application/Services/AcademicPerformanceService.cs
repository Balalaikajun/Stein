using Application.DTOs.AcademicPerformance;
using Application.DTOs.Base;
using Application.DTOs.Department;
using Application.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;
using Domain.Interfaces;
using LinqKit;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class AcademicPerformanceService : IAcademicPerformanceService
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public AcademicPerformanceService(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<BasePaginatedResult<AcademicPerformanceGetDto?>> GetPaginated(
        AcademicPerformancePaginatedRequest request)
    {
        var groupKey = request.GroupKeys.FirstOrDefault()
                       ?? throw new ArgumentException("GroupFilter must contain at least one item.");

        // Базовый запрос с условиями группы и сортировкой
        var baseQuery = _context.AcademicPerformances
            .Where(ap =>
                ap.Student.GroupSpecializationId == groupKey.SpecializationId &&
                ap.Student.GroupYear == groupKey.Year &&
                ap.Student.GroupId == groupKey.Index);

        // Запрос для уникальных месяцев с сортировкой
        var monthQuery = baseQuery
            .Select(ap => new { ap.Year, ap.Month })
            .Distinct()
            .OrderBy(x => x.Year)
            .ThenBy(x => x.Month);

        // Применяем пагинацию к месяцам
        var months = await monthQuery
            .Skip(request.Skip)
            .Take(request.Take + 1)
            .ToListAsync();

        var hasMore = months.Count > request.Take;
        var pagedMonths = months.Take(request.Take).ToList();

        if (!pagedMonths.Any())
            return new BasePaginatedResult<AcademicPerformanceGetDto?>(
                Enumerable.Empty<AcademicPerformanceGetDto?>(),
                false,
                0);

        // Собираем предикат для выбора записей по отфильтрованным месяцам
        var predicate = PredicateBuilder.New<AcademicPerformance>();
        foreach (var m in pagedMonths) predicate = predicate.Or(ap => ap.Year == m.Year && ap.Month == m.Month);

        // Получаем данные с учетом пагинации по месяцам
        var items = await baseQuery
            .Where(predicate)
            .OrderByDescending(ap => ap.Year)
            .ThenByDescending(ap => ap.Month)
            .ProjectTo<AcademicPerformanceGetDto?>(_mapper.ConfigurationProvider)
            .ToListAsync();

        // Общее количество месяцев (для корректной пагинации)
        var total = await monthQuery.CountAsync();

        return new BasePaginatedResult<AcademicPerformanceGetDto?>(
            items,
            hasMore,
            total);
    }
}