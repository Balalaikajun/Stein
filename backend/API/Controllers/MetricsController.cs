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

    [HttpGet("StudentsCount")]
    public async Task<IActionResult> GetStudentsCountAsync()
    {
        return Ok(await _metricsService.GetStudentsCountAsync());
    }
    
    [HttpGet("OrdersCount")]
    public async Task<IActionResult> GetOrdersCountAsync()
    {
        return Ok(await _metricsService.GetOrdersCountAsync());
    }
    
    [HttpGet("ForeignersCount")]
    public async Task<IActionResult> GetForeignersCountAsync()
    {
        return Ok(await _metricsService.GetForeignersCountAsync());
    }
}