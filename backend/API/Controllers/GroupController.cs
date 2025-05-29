using Application.DTOs.Base;
using Application.DTOs.Department;
using Application.DTOs.Group;
using Application.Interfaces;
using Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GroupController : ControllerBase
{
    private readonly IGroupService _groupService;

    public GroupController(IGroupService groupService)
    {
        _groupService = groupService;
    }

    [HttpPost("filter")]
    [Authorize]
    public async Task<ActionResult<BasePaginatedResult<GroupGetDto>>> GetPaginated(GroupPaginatedRequest request)
    {
        var result = await _groupService.GetPaginated(request);

        return Ok(result);
    }

    // [HttpPost]
    // [Authorize]
    // public async Task<ActionResult> Create(GroupPostDto dto)
    // {
    //     try
    //     {
    //         await _groupService.Create(dto);
    //         return Created();
    //     }
    //     catch (Exception e)
    //     {
    //         Console.WriteLine(e);
    //         return BadRequest("Неизвестная ошибка");
    //     }
    // }

    [HttpPatch]
    [Authorize]
    public async Task<ActionResult> Patch(GroupPatchDto dto)
    {
        try
        {
            await _groupService.Update(dto);
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
    public async Task<ActionResult> Delete([FromQuery] GroupKeyDto id)
    {
        try
        {
            await _groupService.Delete(id);
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