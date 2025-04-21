using Application.DTOs.Department;
using Application.DTOs.Teacher;
using Application.Interfaces;
using Application.Services;
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

    [HttpPost]
    [Authorize]
    public async Task<BasePaginatedResult<TeacherGetDto>> GetTeachers(TeacherPaginatedRequest request)
    {
        var result = await _teacherService.GetPaginated(request);
        
        return result;
    }
    
}