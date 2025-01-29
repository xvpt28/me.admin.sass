using me.admin.api.DTOs;
using me.admin.api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace me.admin.api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrderController(OrderService orderService) : ControllerBase
{
	readonly OrderService _orderService = orderService;
	[Authorize]
	[HttpGet("all/ongoing/{outletId}")]
	public async Task<IActionResult> GetAllOngoingOrder([FromRoute] string outletId)
	{
		var response = await _orderService.GetAllOngoingOrder(outletId);
		if (response.Success)
			return Ok(response);
		return Unauthorized(response);
	}

	[Authorize]
	[HttpGet("all/filter/{outletId}")]
	public async Task<IActionResult> GetAllOrderWithFilter([FromRoute] string outletId, [FromQuery] GetOrderFilterDto query)
	{
		var response = await _orderService.GetAllOrdersWithFilters(outletId, query);
		if (response.Success)
			return Ok(response);
		return Unauthorized(response);
	}

	[Authorize]
	[HttpGet("{orderId}")]
	public async Task<IActionResult> GetOrderById([FromRoute] string orderId)
	{
		var response = await _orderService.GetOrderById(orderId);
		if (response.Success)
			return Ok(response);
		return Unauthorized(response);
	}

	[Authorize]
	[HttpPost("create/{outletId}")]
	public async Task<IActionResult> CreateOrder(
		[FromRoute] string outletId,
		[FromBody] CreateOrderDto body
	)
	{
		var response = await _orderService.CreateOrder(outletId, body);
		if (response.Success)
			return Ok(response);
		return Unauthorized(response);
	}

	[Authorize]
	[HttpPut("update/{orderId}")]
	public async Task<IActionResult> UpdateOrder(
		[FromRoute] string orderId,
		[FromBody] UpdateOrderDto body
	)
	{
		var response = await _orderService.UpdateOrder(orderId, body);
		if (response.Success)
			return Ok(response);
		return Unauthorized(response);
	}

	[Authorize]
	[HttpDelete("{orderId}")]
	public async Task<IActionResult> DeleteOrder([FromRoute] string orderId)
	{
		var response = await _orderService.DeleteOrder(orderId);
		if (response.Success)
			return Ok(response);
		return Unauthorized(response);
	}
}