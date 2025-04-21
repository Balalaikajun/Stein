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

    [HttpPost]
    [Authorize]
    public async Task<ActionResult> GetDepartments(DepartmentPaginatedRequestDto request)
    {
        var result = await _departmentService.GetPaginated(request);
        
        return Ok(result);
    }
    
}