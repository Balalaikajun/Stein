using Application.DTOs.Metrics;
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

    public async Task<CountDto> GetStudentCountAsync()
    {
        return new CountDto( await _context.Students
            .Where(s => s.Status == StudentStatuses.Active)
            .CountAsync());
    }

    public async Task<CountDto> GetOrderCountAsync()
    {
        var today = DateTime.Now;
        return new CountDto(await _context.Orders
            .Where(o => o.Date > new DateOnly(today.Year, today.Month, 1))
            .CountAsync());
    }

    public async Task<CountDto> GetForeignCountAsync()
    {
        return new CountDto( await _context.Students
            .Where(s => s.IsCitizen == false)
            .CountAsync());
    }

    public async Task<CountsDto> GetAllCountsAsync()
    {
        var studentsTask =await GetStudentCountAsync();
        var ordersTask =await GetOrderCountAsync();
        var foreignersTask = await GetForeignCountAsync();


        return new CountsDto(
             studentsTask,
             ordersTask,
             foreignersTask
        );
    }
    
    public async Task<PieDto> GetGenderDistributionAsync()
    {
        var segments = await _context.Students
            .GroupBy(s => s.Gender)
            .Select(g => new PieSegment(
                g.Key.ToString(),
                g.Count()))
            .ToListAsync();
        
        return new PieDto(segments);
    }
    
    public async Task<PieDto> GetAgeDistributionAsync()
    {
        var today = DateTime.Now;
        var date17  = DateOnly.FromDateTime(today.AddYears(-17));  
        var date21  = DateOnly.FromDateTime(today.AddYears(-21));   
        var date26  = DateOnly.FromDateTime(today.AddYears(-26)); 
        
        var segments = await _context.Students
            .AsNoTracking()
            .GroupBy(s => 
                s.DateOfBirth > date21 && s.DateOfBirth <= date17 ? "17–20" :
                s.DateOfBirth > date26 && s.DateOfBirth <= date21 ? "21–25" :
                "26+")
            .Select(g => new PieSegment(
                g.Key.ToString(),
                g.Count()))
            .ToListAsync();
        
        return new PieDto(segments);
    }
    public async Task<PieDto> GetCitizenshipDistributionAsync()
    {
        var segments = await _context.Students
            .GroupBy(s => s.IsCitizen)
            .Select(g => new PieSegment(
                g.Key.ToString(),
                g.Count()
            ))
            .ToListAsync();

        return new PieDto(segments);
    }

    public async Task<PiesDto> GetAllPieDistributionsAsync()
    {
        var genderTask =await GetGenderDistributionAsync();
        var ageTask =await GetAgeDistributionAsync();
        var citizenshipTask = await GetCitizenshipDistributionAsync();


        return new PiesDto(
             genderTask,
             ageTask,
             citizenshipTask
        );
    }
}