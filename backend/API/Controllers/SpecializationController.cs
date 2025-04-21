using Application.DTOs.Specialization;
using Application.Interfaces;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SpecializationController:ControllerBase
{
    private readonly ISpecializationService _specializationService;

    public SpecializationController(ISpecializationService specializationService)
    {
        _specializationService = specializationService;
    }

    [HttpPost]
    public async Task<ActionResult> GetSpecializations(SpecializationPaginatedRequest request)
    {
        var result = await _specializationService.GetPaginated(request);
        
        return Ok(result);
    }
}