using me.admin.api.DTOs;
using me.admin.api.Services;
using Microsoft.AspNetCore.Mvc;

namespace me.admin.api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OutletController(OutletService outletService) : ControllerBase
{
	readonly OutletService _outletService = outletService;

	[HttpPost("new")]
	public async Task<IActionResult> CreateOutlet([FromBody] CreateOutletRequestDto outletDto)
	{
		var response = await _outletService.CreateOutlet(outletDto);

		if (response.Success)
			return Ok(response);
		return Unauthorized(response);
	}

	[HttpGet("{outletId}")]
	public async Task<IActionResult> GetOutletById([FromRoute] string outletId)
	{
		var response = await _outletService.GetOutletById(outletId);

		if (response.Success)
			return Ok(response);
		return Unauthorized(response);
	}

	[HttpGet("all")]
	public async Task<IActionResult> GetAllOutlets()
	{
		var response = await _outletService.GetAllOutlets();

		if (response.Success)
			return Ok(response);
		return Unauthorized(response);
	}
}