using Application.DTOs.Order;
using Application.Interfaces;
using Application.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrderController: ControllerBase
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpPost]
    public async Task<ActionResult> GetPaginated(OrderPaginatedRequest request)
    {
        var result =await _orderService.GetPaginated(request);
        
        return Ok(result);
    }
    
    
}