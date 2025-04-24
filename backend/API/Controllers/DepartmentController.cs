using System.Text.Json;
using Application.DTOs.Department;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DepartmentController: ControllerBase
{
    private readonly IDepartmentService _departmentService;

    public DepartmentController(IDepartmentService departmentService)
    {
        _departmentService = departmentService;
    }

    [HttpPost("filter")]
    [Authorize]
    public async Task<ActionResult> GetDepartments(DepartmentPaginatedRequest request)
    {
        var result = await _departmentService.GetPaginated(request);
        
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult> Create(DepartmentPostDto dto)
    {
        try
        {
            await _departmentService.Create(dto);
            return Created();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return BadRequest("Неизвестная ошибка");
        }
        
        
    }
    
    
}