using System.Linq.Dynamic.Core;
using Application.DTOs.Metrics;
using Application.Enums.MetricTypes;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using LinqKit;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class KpiService : IKpiService
{
    private readonly IApplicationDbContext _context;
    private readonly Dictionary<KpiTypes, Func<KpiRequest, Task<CountDto>>> _handlers;

    public KpiService(IApplicationDbContext context)
    {
        _context = context;

        _handlers = new Dictionary<KpiTypes, Func<KpiRequest, Task<CountDto>>>
        {
            { KpiTypes.Students, GetStudentKpiAsync },
            { KpiTypes.Orders, GetOrderKpiAsync },
            { KpiTypes.Foreigners, GetForeignKpiAsync }
        };
    }

    public async Task<CountDto> GetKpiAsync(KpiTypes types, KpiRequest request)
    {
        if(!_handlers.ContainsKey(types))
            throw new InvalidOperationException($"No handler for '{types}' is registered.");
        return await _handlers[types](request);
    }

    private async Task<CountDto> GetStudentKpiAsync(KpiRequest request)
    {
        var quarry = _context.Students
            .AsNoTracking();

        quarry = quarry.Where(s => s.Status == StudentStatuses.Active);

        return new CountDto(await quarry.CountAsync());
    }

    private async Task<CountDto> GetOrderKpiAsync(KpiRequest request)
    {
        var quarry = _context.Orders.AsNoTracking();

        var today = DateTime.Now;
        quarry = quarry.Where(o => o.Date > new DateOnly(today.Year, today.Month, 1));

        return new CountDto(await quarry.CountAsync());
    }

    private async Task<CountDto> GetForeignKpiAsync(KpiRequest request)
    {
        var quarry = _context.Students.AsNoTracking();

        quarry = quarry
            .Where(s => s.IsCitizen == false && s.Status == StudentStatuses.Active);
        
        return new CountDto(await _context.Students
            .CountAsync());
    }
}