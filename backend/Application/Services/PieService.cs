using Application.DTOs.Metrics;
using Application.Enums.MetricTypes;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using LinqKit;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class PieService : IPieService
{
    private readonly IApplicationDbContext _context;
    private readonly Dictionary<PieTypes, Func<PieRequest, Task<PieDto>>> _handlers;

    public PieService(IApplicationDbContext context)
    {
        _context = context;

        _handlers = new Dictionary<PieTypes, Func<PieRequest, Task<PieDto>>>
        {
            { PieTypes.Gender, GetGenderPieAsync },
            { PieTypes.AgeGroup, GetAgePieAsync },
            { PieTypes.Nationality, GetNationalityPieAsync }
        };
    }

    public async Task<PieDto> GetPieAsync(PieTypes types, PieRequest request)
    {
        if (!_handlers.ContainsKey(types))
            throw new InvalidOperationException($"No handler for '{types}' is registered.");
        return await _handlers[types](request);
    }

    private async Task<PieDto> GetGenderPieAsync(PieRequest request)
    {
        var quarry = _context.Students
            .AsNoTracking();

        if (request.Departments?.Any() == true)
            quarry = quarry.Where(a => request.Departments.Contains(a.Group.Specialization.DepartmentId));

        if (request.Specializations?.Any() == true)
            quarry = quarry.Where(a => request.Specializations.Contains(a.Group.SpecializationId));

        if (request.IsFullTime == true)
            quarry = quarry.Where(a => !a.GroupId.Contains("з"));
        else if (request.IsFullTime == false)
            quarry = quarry.Where(a => a.GroupId.Contains("з"));

        if (request.Groups?.Any() == true)
        {
            var groupPredicate = PredicateBuilder.New<Student>(false);
            foreach (var k in request.Groups)
            {
                groupPredicate = groupPredicate.Or(s =>
                    s.GroupSpecializationId == k.SpecializationId &&
                    s.GroupYear == k.Year &&
                    s.GroupId == k.Index);
            }

            quarry = quarry.Where(groupPredicate);
        }

        var result = await quarry.GroupBy(s => s.Gender)
            .Select(g => new PieSegment(
                g.Key.ToString(),
                g.Count()))
            .ToListAsync();


        return new PieDto(result);
    }

    private async Task<PieDto> GetAgePieAsync(PieRequest request)
    {
        var today = DateTime.Now;
        var date17 = DateOnly.FromDateTime(today.AddYears(-17));
        var date21 = DateOnly.FromDateTime(today.AddYears(-21));
        var date26 = DateOnly.FromDateTime(today.AddYears(-26));

        var quarry = _context.Students
            .AsNoTracking();

        if (request.Departments?.Any() == true)
            quarry = quarry.Where(a => request.Departments.Contains(a.Group.Specialization.DepartmentId));

        if (request.Specializations?.Any() == true)
            quarry = quarry.Where(a => request.Specializations.Contains(a.Group.SpecializationId));

        if (request.IsFullTime == true)
            quarry = quarry.Where(a => !a.GroupId.Contains("з"));
        else if (request.IsFullTime == false)
            quarry = quarry.Where(a => a.GroupId.Contains("з"));

        if (request.Groups?.Any() == true)
        {
            var groupPredicate = PredicateBuilder.New<Student>(false);
            foreach (var k in request.Groups)
            {
                groupPredicate = groupPredicate.Or(s =>
                    s.GroupSpecializationId == k.SpecializationId &&
                    s.GroupYear == k.Year &&
                    s.GroupId == k.Index);
            }

            quarry = quarry.Where(groupPredicate);
        }

        var result = await quarry.GroupBy(s =>
                s.DateOfBirth > date21 && s.DateOfBirth <= date17 ? "17–20" :
                s.DateOfBirth > date26 && s.DateOfBirth <= date21 ? "21–25" :
                "26+")
            .Select(g => new PieSegment(
                g.Key.ToString(),
                g.Count()))
            .ToListAsync();

        return new PieDto(result);
    }

    private async Task<PieDto> GetNationalityPieAsync(PieRequest request)
    {
        var quarry = _context.Students
            .AsNoTracking();

        if (request.Departments?.Any() == true)
            quarry = quarry.Where(a => request.Departments.Contains(a.Group.Specialization.DepartmentId));

        if (request.Specializations?.Any() == true)
            quarry = quarry.Where(a => request.Specializations.Contains(a.Group.SpecializationId));

        if (request.IsFullTime == true)
            quarry = quarry.Where(a => !a.GroupId.Contains("з"));
        else if (request.IsFullTime == false)
            quarry = quarry.Where(a => a.GroupId.Contains("з"));

        if (request.Groups?.Any() == true)
        {
            var groupPredicate = PredicateBuilder.New<Student>(false);
            foreach (var k in request.Groups)
            {
                groupPredicate = groupPredicate.Or(s =>
                    s.GroupSpecializationId == k.SpecializationId &&
                    s.GroupYear == k.Year &&
                    s.GroupId == k.Index);
            }

            quarry = quarry.Where(groupPredicate);
        }

        var result = await quarry.GroupBy(s => s.IsCitizen)
            .Select(g => new PieSegment(
                g.Key.ToString(),
                g.Count()
            ))
            .ToListAsync();

        return new PieDto(result);
    }
}