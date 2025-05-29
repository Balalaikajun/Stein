using Application.DTOs.AcademicPerformance;
using Application.DTOs.Base;
using Application.DTOs.Department;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AcademicPerformanceController : ControllerBase
{
    private readonly IAcademicPerformanceService _academicPerformanceService;

    public AcademicPerformanceController(IAcademicPerformanceService academicPerformanceService)
    {
        _academicPerformanceService = academicPerformanceService;
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<BasePaginatedResult<AcademicPerformanceGetDto>>> Get(
        AcademicPerformancePaginatedRequest request)
    {
        var result = await _academicPerformanceService.GetPaginated(request);

        return Ok(result);
    }
}