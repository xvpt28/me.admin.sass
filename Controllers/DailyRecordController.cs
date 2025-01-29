using me.admin.api.DTOs;
using me.admin.api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace me.admin.api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DailyRecordController(DailyRecordService dailyRecordService) : ControllerBase
{
	readonly DailyRecordService _dailyRecordService = dailyRecordService;

	[Authorize(Roles = "SuperAdmin")]
	[HttpPost("create/admin/{outletId}")]
	public async Task<IActionResult> CreateAdminDailyRecord(
		[FromRoute] string outletId,
		[FromBody] CreateDailyRecordRequestDto body
	)
	{
		var response = await _dailyRecordService.CreateDailyRecord(outletId, body);
		if (response.Success)
			return Ok(response);
		return Unauthorized(response);
	}

	[Authorize]
	[HttpPost("create/{outletId}")]
	public async Task<IActionResult> CreateDailyRecord(
		[FromRoute] string outletId,
		[FromBody] CreateDailyRecordRequestDto body
	)
	{
		var response = await _dailyRecordService.CreateDailyRecord(outletId, body);
		if (response.Success)
			return Ok(response);
		return Unauthorized(response);
	}

	[Authorize]
	[HttpPut("update/{dailyRecordId}")]
	public async Task<IActionResult> UpdateDailyRecord(
		[FromRoute] string dailyRecordId,
		[FromBody] UpdateDailyRecordRequestDto body
	)
	{
		var response = await _dailyRecordService.UpdateDailyRecord(dailyRecordId, body);
		if (response.Success)
			return Ok(response);
		return Unauthorized(response);
	}

	[Authorize(Roles = "SuperAdmin")]
	[HttpGet("all/daily/{outletId}")]
	public async Task<IActionResult> GetAllDailyRecordsByOutlet([FromRoute] string outletId, [FromQuery] GetDailyRecordFilterDto filter)
	{
		var response = await _dailyRecordService.GetAllDailyRecordsByOutletWithFilter(outletId, filter);

		if (response.Success)
			return Ok(response);
		return Unauthorized(response);
	}

	[Authorize(Roles = "SuperAdmin")]
	[HttpGet("all/monthly/{outletId}")]
	public async Task<IActionResult> GetAllMonthlyRecordsByOutlet([FromRoute] string outletId, [FromQuery] GetDailyRecordFilterDto filter)
	{
		var response = await _dailyRecordService.GetAllMonthlyRecordsByOutletWithFilter(outletId, filter);

		if (response.Success)
			return Ok(response);
		return Unauthorized(response);
	}

	[Authorize(Roles = "SuperAdmin")]
	[HttpGet("all/date/{date}")]
	public async Task<IActionResult> GetAllRecordsByDate([FromRoute] long date)
	{
		var response = await _dailyRecordService.GetAllRecordsByDate(date);

		if (response.Success)
			return Ok(response);
		return Unauthorized(response);
	}

	[Authorize]
	[HttpGet("{outletId}/{date}")]
	public async Task<IActionResult> GetAllRecordsByOutletAndDate([FromRoute] string outletId, [FromRoute] long date)
	{
		var response = await _dailyRecordService.GetRecordByOutletAndDate(outletId, date);

		if (response.Success)
			return Ok(response);
		return Unauthorized(response);
	}

	[Authorize(Roles = "SuperAdmin")]
	[HttpDelete("{dailyRecordId}")]
	public async Task<IActionResult> DeleteRecordById([FromRoute] string id)
	{
		var response = await _dailyRecordService.DeleteDailyRecord(id);

		if (response.Success)
			return Ok(response);
		return Unauthorized(response);
	}
}