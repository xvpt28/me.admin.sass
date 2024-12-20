using me.admin.api.Services;
using Microsoft.AspNetCore.Mvc;

namespace me.admin.api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrderController(OrderService orderService) : ControllerBase
{
    readonly OrderService _orderService = orderService;

    [HttpGet("all/{outletId}")]
    public async Task<IActionResult> GetAllOrder([FromRoute] string outletId)
    {
        var response = await _orderService.GetAllOrder(outletId);
        if (response.Success)
            return Ok(response);
        return Unauthorized(response);
    }

    [HttpGet("{orderId}")]
    public async Task<IActionResult> GetOrderById([FromRoute] string orderId)
    {
        var response = await _orderService.GetOrderById(orderId);
        if (response.Success)
            return Ok(response);
        return Unauthorized(response);
    }
}
