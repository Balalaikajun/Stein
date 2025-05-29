using Application.DTOs.Base;
using Application.DTOs.Department;
using Application.Interfaces;
using Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DepartmentController : ControllerBase
{
    private readonly IDepartmentService _departmentService;

    public DepartmentController(IDepartmentService departmentService)
    {
        _departmentService = departmentService;
    }

    [HttpPost("filter")]
    [Authorize]
    public async Task<ActionResult<BasePaginatedResult<DepartmentGetDto>>> GetDepartments(
        DepartmentPaginatedRequest request)
    {
        var result = await _departmentService.GetPaginated(request);

        return Ok(result);
    }

    // [HttpPost]
    // [Authorize]
    // public async Task<ActionResult> Create(DepartmentPostDto dto)
    // {
    //     try
    //     {
    //         await _departmentService.Create(dto);
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
    public async Task<ActionResult> Patch(DepartmentPatchDto dto)
    {
        try
        {
            await _departmentService.Update(dto);
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
            await _departmentService.Delete(id);
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