using Application.DTOs.Base;
using Application.DTOs.Department;
using Application.DTOs.Order;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [Authorize]
    [HttpPost("filter")]
    public async Task<ActionResult<BasePaginatedResult<OrderGetDto>>> GetPaginated(OrderPaginatedRequest request)
    {
        var result = await _orderService.GetPaginated(request);

        return Ok(result);
    }
    //
    // [HttpPost]
    // [Authorize]
    // public async Task<ActionResult> Create(OrderPostDto dto)
    // {
    //     try
    //     {
    //         await _orderService.Create(dto);
    //         return Created();
    //     }
    //     catch (Exception e)
    //     {
    //         Console.WriteLine(e);
    //         return BadRequest(e.InnerException.Message);
    //     }
    // }

    [HttpDelete]
    [Authorize]
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            await _orderService.Delete(id);
            return NoContent();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return BadRequest(e.Message);
        }
    }
}