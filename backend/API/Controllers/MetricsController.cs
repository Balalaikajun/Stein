using Application.DTOs.Metrics;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MetricsController:ControllerBase
{
    private readonly IMetricsService _metricsService;

    public MetricsController(IMetricsService metricsService)
    {
        _metricsService = metricsService;
    }

    [HttpGet("counts/students")]
    public async Task<ActionResult<CountDto>> GetStudentCountAsync()
    {
        var dto = await _metricsService.GetStudentCountAsync();
        return Ok(dto);
    }
    
    [HttpGet("counts/orders")]
    public async Task<ActionResult<CountDto>> GetOrderCountAsync()
    {
        var dto = await _metricsService.GetOrderCountAsync();
        return Ok(dto);
    }
    
    [HttpGet("counts/foreigners")]
    public async Task<ActionResult<CountDto>> GetForeignCountAsync()
    {
        var dto = await _metricsService.GetForeignCountAsync();
        return Ok(dto);
    }
    [HttpGet("counts")]
    public async Task<ActionResult<CountsDto>> GetAllCountsAsync()
    {
        var dto = await _metricsService.GetAllCountsAsync();
        return Ok(dto);
    }
    
    [HttpGet("pies/gender")]
    public async Task<ActionResult<PieDto>> GetGenderDistributionAsync()
    {
        var dto = await _metricsService.GetGenderDistributionAsync();
        return Ok(dto);
    }
    
    [HttpGet("pies/age")]
    public async Task<ActionResult<PieDto>> GetAgeDistributionAsync()
    {
        var dto = await _metricsService.GetAgeDistributionAsync();
        return Ok(dto);
    }
    
    [HttpGet("pies/citizenship")]
    public async Task<ActionResult<PieDto>> GetCitizenshipDistributionAsync()
    {
        var dto = await _metricsService.GetCitizenshipDistributionAsync();
        return Ok(dto);
    }
    
    [HttpGet("pies")]
    public async Task<ActionResult<IEnumerable<PieDto>>> GetAllPieDistributionsAsync()
    {
        var dto = await _metricsService.GetAllPieDistributionsAsync();
        return Ok(dto);
    }
}