using Application.DTOs.AcademicPerformance;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AcademicPerformanceController: ControllerBase
{
    private readonly IAcademicPerformanceService _academicPerformanceService;

    public AcademicPerformanceController(IAcademicPerformanceService academicPerformanceService)
    {
        _academicPerformanceService = academicPerformanceService;
    }

    [HttpPost]
    public async Task<IActionResult> Post(AcademicPerformancePaginatedRequest request)
    {
        var result = await _academicPerformanceService.GetPaginated(request);
        
        return Ok(result);
    }
    
}