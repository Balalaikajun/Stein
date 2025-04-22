using Application.DTOs.Student;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StudentController:ControllerBase
{
    private readonly IStudentService _studentService;

    public StudentController(IStudentService studentService)
    {
        _studentService = studentService;
    }

    [HttpPost]
    public async Task<IActionResult> GetStudents(StudentPaginatedRequest request)
    {
        var result = await _studentService.GetPaginated(request);
        
        return Ok(result);
    }
    
}