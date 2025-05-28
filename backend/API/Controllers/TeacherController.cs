using Application.DTOs.Department;
using Application.DTOs.Teacher;
using Application.Interfaces;
using Application.Services;
using Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TeacherController: ControllerBase
{
    private readonly ITeacherService _teacherService;

    public TeacherController(ITeacherService teacherService)
    {
        _teacherService = teacherService;
    }

    [HttpPost("filter")]
    [Authorize]
    public async Task<BasePaginatedResult<TeacherGetDto>> GetTeachers(TeacherPaginatedRequest request)
    {
        var result = await _teacherService.GetPaginated(request);
        
        return result;
    }
    
    /*[HttpPost]
    [Authorize]
    public async Task<ActionResult> Create(TeacherPostDto dto)
    {
        try
        {
            await _teacherService.Create(dto);
            return Created();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return BadRequest("Неизвестная ошибка");
        }
        
        
    }*/
    
    [HttpPatch]
    [Authorize]
    public async Task<ActionResult> Patch(TeacherPatchDto dto)
    {
        try
        {
            await _teacherService.Update(dto);
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
            await _teacherService.Delete(id);
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