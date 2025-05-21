using System.Linq.Dynamic.Core;
using Application.DTOs.Metrics;
using Application.Interfaces;
using AutoMapper.Execution;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace Application.Services;

public class UniqueChartsService : IUniqueChartsService
{
    private IApplicationDbContext _context;

    public UniqueChartsService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PerformanceHistogramDto> GetPerformanceHistogramAsync(PerformanceRequest request)
    {
        var fromYm = request.YearFrom * 100 + request.MonthFrom;
        var toYm = request.YearTo * 100 + request.MonthTo;

        var quarry = _context.AcademicPerformances
            .AsNoTracking();

        if (request.Departments?.Any() == true)
            quarry = quarry.Where(a => request.Departments.Contains(a.Student.Group.Specialization.DepartmentId));

        if (request.Specializations?.Any() == true)
            quarry = quarry.Where(a => request.Specializations.Contains(a.Student.Group.SpecializationId));

        if (request.Groups?.Any() == true)
        {
            var groupPredicate = PredicateBuilder.New<AcademicPerformance>(false);
            foreach (var k in request.Groups)
            {
                groupPredicate = groupPredicate.Or(s =>
                    s.Student.GroupSpecializationId == k.SpecializationId &&
                    s.Student.GroupYear == k.Year &&
                    s.Student.GroupId == k.Index);
            }

            quarry = quarry.Where(groupPredicate);
        }

        if (request.IsFullTime == true)
            quarry = quarry.Where(a => !a.Student.GroupId.Contains("з"));
        else if (request.IsFullTime == false)
            quarry = quarry.Where(a => a.Student.GroupId.Contains("з"));

        var hasBefore = await quarry
            .AnyAsync(p => (p.Year * 100 + p.Month) < fromYm);
        var hasAfter = await quarry
            .AnyAsync(p => (p.Year * 100 + p.Month) > toYm);

        var categories = Enum.GetValues<PerformanceCategory>();

        var raw = await quarry
            .Where(p => (p.Year * 100 + p.Month) >= fromYm
                        && (p.Year * 100 + p.Month) <= toYm)
            .GroupBy(p => new { p.Year, p.Month })
            .Select(g => new
            {
                g.Key.Year,
                g.Key.Month,
                TotalCount = g.Count(),
                ByCategory = g.GroupBy(x => x.Performance)
                    .Select(gc => new { Category = gc.Key, Count = gc.Count() })
                    .ToList()
            })
            .OrderBy(x => x.Year).ThenBy(x => x.Month)
            .ToListAsync();

        var arr = raw.Select(x =>
        {
            var dict = categories.ToDictionary(
                c => c,
                c => x.ByCategory.FirstOrDefault(b => b.Category == c)?.Count ?? 0
            );
            return new PerformanceElementDto(x.Year, x.Month, x.TotalCount, dict);
        }).ToList();


        var result = new PerformanceHistogramDto(
            arr,
            hasBefore,
            hasAfter
        );

        return result;
    }

    public async Task<OrderHistogramDto> GetOrderHistogramAsync(OrderHistogramRequest request)
    {
        var query = _context.Orders
            .AsNoTracking();

        var now = DateOnly.FromDateTime(DateTime.Now);
        var start = request.DateRange.FromDate.GetValueOrDefault(new DateOnly(now.Year-1, now.Month, now.Day));
        var end = request.DateRange.FromDate.GetValueOrDefault(new DateOnly(now.Year, now.Month, now.Day));
        
        query = query.Where(o => o.Date >= start && o.Date <= end);
        
        // Фильтр по кафедрам через специализацию
        if (request.Departments?.Any() == true)
        {
            query = query.Where(a =>
                (a.FromGroup.Specialization.DepartmentId != null &&
                 request.Departments.Contains(a.FromGroup.Specialization.DepartmentId)) ||
                (a.ToGroup.Specialization.DepartmentId != null &&
                 request.Departments.Contains(a.ToGroup.Specialization.DepartmentId))
            );
        }

        // Фильтр по специализациям
        if (request.Specializations?.Any() == true)
        {
            var specs = request.Specializations.Cast<int?>();
            query = query.Where(a =>
                specs.Contains(a.FromSpecializationId) ||
                specs.Contains(a.ToSpecializationId)
            );
        }

        if (request.Groups?.Any() == true)
        {
            var groupPredicate = PredicateBuilder.New<Group>(false);
            foreach (var k in request.Groups)
            {
                groupPredicate = groupPredicate.Or(s =>
                    s.SpecializationId == k.SpecializationId &&
                    s.Year == k.Year &&
                    s.Index == k.Index);
            }

            query = query.Where(groupPredicate);
        }

        // Фильтр по форме обучения
        if (request.IsFullTime == true)
        {
            query = query.Where(a =>
                !a.FromGroupId.Contains("з") &&
                !a.ToGroupId.Contains("з")
            );
        }
        else if (request.IsFullTime == false)
        {
            query = query.Where(a =>
                a.FromGroupId.Contains("з") ||
                a.ToGroupId.Contains("з")
            );
        }
        
        var count = await query.CountAsync();
        var data = await query
            .GroupBy(o => o.OrderType)
            .Select(g => new { Category = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Category, x => x.Count);

        return new OrderHistogramDto(count, data);
    }

    public async Task<ContingentHistogramDto> GetContingentHistogramAsync(ContingentHistogramRequest request)
    {
        var query = _context.Groups
            .AsNoTracking();

        // Фильтр по форме обучения
        if (request.IsFullTime == true)
        {
            query = query.Where(a => !a.Index.Contains("з"));
        }
        else if (request.IsFullTime == false)
        {
            query = query.Where(a =>
                a.Index.Contains("з")
            );
        }

        var nowDate = DateOnly.FromDateTime(DateTime.Now);
        
        var raw = await query
            .Where(g =>
                g.Year <= request.Date.Year
                && (g.ReleaseDate.HasValue
                    ? g.ReleaseDate.Value
                    : nowDate) >= request.Date
            )
            .OrderBy(g => g.Specialization.Department.Title).ThenBy(g => g.Specialization.Title)
            .Select(g => new {
                Department=  g.Specialization.Department.Title,
                Specialization=g.Specialization.Title,
                Group = g.Acronym,
                Course = g.IsActive ? (request.Date.Year - g.Year).ToString() : "Выпускники",
                Count = g.Students.Count
                - g.EnrollmentOrders.Count(o => o.Date >= request.Date)
                + g.ExplitOrders.Count(o => o.Date >= request.Date)
            })
            .ToListAsync();

        var arr = raw.Select(g => new ContingentDto(
            g.Department,
            g.Specialization,
            g.Course,
            g.Group,
            g.Count
        ));

        return new ContingentHistogramDto(arr);
    }
}