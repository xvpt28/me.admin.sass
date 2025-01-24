using me.admin.api.DTOs;
using me.admin.api.Services;
using Microsoft.AspNetCore.Mvc;

namespace me.admin.api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ImportanceController(ImportanceService importanceService) : ControllerBase
{
	readonly ImportanceService _importanceService = importanceService;

	[HttpPost("create/{outletId}")]
	public async Task<IActionResult> CreateImportance([FromRoute] string outletId, [FromBody] CreateImportanceDto body)
	{
		var response = await _importanceService.CreateImportanceById(outletId, body);
		if (response.Success)
			return Ok(response);
		return NotFound(response);
	}

	[HttpGet("all/{outletId}")]
	public async Task<IActionResult> GetAllImportance([FromRoute] string outletId)
	{
		var response = await _importanceService.GetAllImportanceByOutlet(outletId);
		if (response.Success)
			return Ok(response);
		return NotFound(response);
	}

	[HttpPut("update/{noteId}")]
	public async Task<IActionResult> UpdateImportanceById(
		[FromRoute] string noteId,
		[FromBody] UpdateImportanceDto body
	)
	{
		var response = await _importanceService.UpdateImportanceById(noteId, body);
		if (response.Success)
			return Ok(response);
		return NotFound(response);
	}

	[HttpDelete("{noteId}")]
	public async Task<IActionResult> DeleteMenuById([FromRoute] string noteId)
	{
		var response = await _importanceService.DeleteImportance(noteId);
		if (response.Success)
			return Ok(response);
		return NotFound(response);
	}
}