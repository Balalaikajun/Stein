using Application.Interfaces;
using Domain.Enums;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class MetricsService: IMetricsService
{
    private IApplicationDbContext _context;

    public MetricsService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> GetStudentsCountAsync()
    {
        return await _context.Students
            .Where(s => s.Status == StudentStatuses.Active)
            .CountAsync();
    }

    public async Task<int> GetOrdersCountAsync()
    {
        var today = DateTime.Now;
        return await _context.Orders
            .Where(o => o.Date > new DateOnly(today.Year, today.Month, 1))
            .CountAsync();
    }

    public async Task<int> GetForeignersCountAsync()
    {
        return await _context.Students
            .Where(s => s.IsCitizen == false)
            .CountAsync();
    }
    
}