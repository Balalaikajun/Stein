using Application.DTOs.Department;
using Application.DTOs.Group;
using Application.Interfaces;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GroupController: ControllerBase
{
    private readonly IGroupService _groupService;

    public GroupController(IGroupService groupService)
    {
        _groupService = groupService;
    }

    [HttpPost]
    public async Task<IActionResult> GetPaginated(GroupPaginatedRequest request)
    {
        var result = await _groupService.GetPaginated(request);
        
        return Ok(result);
    }
    
}