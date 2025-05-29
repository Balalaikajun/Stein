using Application.DTOs.Metrics;
using Application.Enums.MetricTypes;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MetricsController : ControllerBase
{
    private readonly IKpiService _kpiService;
    private readonly IPieService _pieService;
    private readonly IUniqueChartsService _uniqueChartsService;

    public MetricsController(
        IKpiService kpiService,
        IPieService pieService,
        IUniqueChartsService uniqueChartsService)
    {
        _kpiService = kpiService;
        _pieService = pieService;
        _uniqueChartsService = uniqueChartsService;
    }

    [HttpPost("kpi")]
    [Authorize]
    public async Task<ActionResult<CountDto>> GetKpiAsync(
        [FromQuery] KpiTypes type,
        [FromBody] KpiRequest request)
    {
        try
        {
            var dto = await _kpiService.GetKpiAsync(type, request);

            return Ok(dto);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("pie")]
    [Authorize]
    public async Task<ActionResult<CountDto>> GetPisAsync(
        [FromQuery] PieTypes type,
        [FromBody] PieRequest request)
    {
        try
        {
            var dto = await _pieService.GetPieAsync(type, request);

            return Ok(dto);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }


    [HttpPost("histogram/performance")]
    [Authorize]
    public async Task<ActionResult<PerformanceHistogramDto>> GetPerformanceHistogramAsync(
        [FromBody] PerformanceRequest request)
    {
        try
        {
            var dto = await _uniqueChartsService.GetPerformanceHistogramAsync(request);

            return Ok(dto);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }


    [HttpPost("histogram/order")]
    [Authorize]
    public async Task<ActionResult<OrderHistogramDto>> GetOrderHistogramAsync(OrderHistogramRequest request)
    {
        try
        {
            var dto = await _uniqueChartsService.GetOrderHistogramAsync(request);

            return Ok(dto);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("histogram/contingent")]
    [Authorize]
    public async Task<ActionResult<ContingentDto>> GetOrderContingentAsync(ContingentHistogramRequest request)
    {
        var dto = await _uniqueChartsService.GetContingentHistogramAsync(request);

        return Ok(dto);
    }
}