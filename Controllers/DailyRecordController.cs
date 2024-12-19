using me.admin.api.DTOs;
using me.admin.api.Services;
using Microsoft.AspNetCore.Mvc;

namespace me.admin.api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DailyRecordController(DailyRecordService dailyRecordService) : ControllerBase
{

	readonly DailyRecordService _dailyRecordService = dailyRecordService;

	[HttpPost("new/{outletId}")]
	public async Task<IActionResult> CreateOutlet([FromRoute] string outletId, [FromBody] CreateDailyRecordRequestDto body)
	{
		var response = await _dailyRecordService.CreateDailyRecord(outletId, body);
		if (response.Success)
			return Ok(response);
		return Unauthorized(response);
	}

	[HttpGet("all/{outletId}")]
	public async Task<IActionResult> GetAllOutlets([FromRoute] string outletId)
	{
		var response = await _dailyRecordService.GetAllRecordsByOutlet(outletId);

		if (response.Success)
			return Ok(response);
		return Unauthorized(response);
	}

	[HttpGet("all/date/{date}")]
	public async Task<IActionResult> GetAllOutlets([FromRoute] long date)
	{
		var response = await _dailyRecordService.GetAllRecordsByDate(date);

		if (response.Success)
			return Ok(response);
		return Unauthorized(response);
	}
}