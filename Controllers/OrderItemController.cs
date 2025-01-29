using me.admin.api.DTOs;
using me.admin.api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace me.admin.api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrderItemController(OrderItemService orderItemService) : ControllerBase
{
	readonly OrderItemService _orderItemService = orderItemService;

	[Authorize]
	[HttpGet("all/{orderId}")]
	public async Task<IActionResult> GetAllOrderItems(
		[FromRoute] string orderId
	)
	{
		var response = await _orderItemService.GetAllOrderItems(orderId);
		if (response.Success)
			return Ok(response);
		return Unauthorized(response);
	}

	[Authorize]
	[HttpPost("create/{orderId}")]
	public async Task<IActionResult> CreateOrderItem(
		[FromRoute] string orderId,
		[FromBody] CreateOrderItemDto body
	)
	{
		var response = await _orderItemService.CreateOrderItem(orderId, body);
		if (response.Success)
			return Ok(response);
		return Unauthorized(response);
	}

	[Authorize]
	[HttpPut("update/{orderItemId}")]
	public async Task<IActionResult> UpdateOrder(
		[FromRoute] string orderItemId,
		[FromBody] UpdateOrderItemDto body
	)
	{
		var response = await _orderItemService.UpdateOrderItem(orderItemId, body);
		if (response.Success)
			return Ok(response);
		return Unauthorized(response);
	}

	[Authorize]
	[HttpDelete("{orderItemId}")]
	public async Task<IActionResult> DeleteOrder([FromRoute] string orderItemId)
	{
		var response = await _orderItemService.DeleteOrderItem(orderItemId);
		if (response.Success)
			return Ok(response);
		return Unauthorized(response);
	}
}