using Application.DTOs.Base;
using Application.DTOs.Department;
using Application.DTOs.Student;
using Application.Interfaces;
using Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StudentController : ControllerBase
{
    private readonly IStudentService _studentService;

    public StudentController(IStudentService studentService)
    {
        _studentService = studentService;
    }

    [HttpPost("filter")]
    public async Task<ActionResult<BasePaginatedResult<StudentGetDto>>> GetStudents(StudentPaginatedRequest request)
    {
        var result = await _studentService.GetPaginated(request);

        return Ok(result);
    }
    // [HttpPost]
    // [Authorize]
    // public async Task<ActionResult> Create(StudentPostDto dto)
    // {
    //     try
    //     {
    //         await _studentService.Create(dto);
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
    public async Task<ActionResult> Patch(StudentPatchDto dto)
    {
        try
        {
            await _studentService.Update(dto);
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
            await _studentService.Delete(id);
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