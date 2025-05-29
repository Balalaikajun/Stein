using Application.DTOs.Base;
using Application.DTOs.Department;
using Application.DTOs.Specialization;
using Application.Interfaces;
using Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SpecializationController : ControllerBase
{
    private readonly ISpecializationService _specializationService;

    public SpecializationController(ISpecializationService specializationService)
    {
        _specializationService = specializationService;
    }

    [HttpPost("filter")]
    [Authorize]
    public async Task<ActionResult<BasePaginatedResult<SpecializationGetDto>>> GetSpecializations(
        SpecializationPaginatedRequest request)
    {
        var result = await _specializationService.GetPaginated(request);

        return Ok(result);
    }

    // [HttpPost]
    // [Authorize]
    // public async Task<ActionResult> Create(SpecializationPostDto dto)
    // {
    //     try
    //     {
    //         await _specializationService.Create(dto);
    //         return Created();
    //     }
    //     catch (Exception e)
    //     {
    //         Console.WriteLine(e);
    //         return BadRequest("Неизвестная ошибка");
    //     }
    //     
    //     
    // }

    [HttpPatch]
    [Authorize]
    public async Task<ActionResult> Patch(SpecializationPatchDto dto)
    {
        try
        {
            await _specializationService.Update(dto);
            return Ok();
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return BadRequest("Неизвестная ошибка");
        }
    }

    [HttpDelete]
    [Authorize]
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            await _specializationService.Delete(id);
            return NoContent();
        }
        catch (NotFoundException e)
        {
            return NoContent();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return BadRequest("Неизвестная ошибка");
        }
    }
}