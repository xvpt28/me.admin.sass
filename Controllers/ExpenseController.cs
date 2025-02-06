using me.admin.api.DTOs;
using me.admin.api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace me.admin.api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ExpenseController(ExpenseService expenseService) : ControllerBase
{
    readonly ExpenseService _expenseService = expenseService;

    [Authorize]
    [HttpGet("all/{outletId}/{date}")]
    public async Task<IActionResult> GetAllExpenseRecords(
        [FromRoute] string outletId,
        [FromRoute] long date
    )
    {
        DateTimeOffset dateTime = DateTimeOffset.FromUnixTimeMilliseconds(date).ToLocalTime();
        DateTimeOffset start = dateTime.Date;
        DateTimeOffset end = start.AddDays(1).AddMilliseconds(-1);

        long startTimestamp = start.ToUnixTimeMilliseconds();
        long endTimestamp = end.ToUnixTimeMilliseconds();

        var response = await _expenseService.GetAllExpenseRecordsByOutletAndDate(
            outletId,
            startTimestamp,
            endTimestamp,
            false,
            "cash"
        );
        if (response.Success)
            return Ok(response);
        return NotFound(response);
    }

    [Authorize]
    [HttpPost("create/{outletId}")]
    public async Task<IActionResult> CreateExpenseRecord(
        [FromRoute] string outletId,
        [FromBody] CreateExpenseRecordRequestDto body
    )
    {
        var response = await _expenseService.CreateExpenseRecord(outletId, body);
        if (response.Success)
            return Ok(response);
        return NotFound(response);
    }

    [Authorize(Roles = "SuperAdmin")]
    [HttpPut("update/{expenseId}")]
    public async Task<IActionResult> UpdateImportanceById(
        [FromRoute] string expenseId,
        [FromBody] UpdateExpenseRecordRequestDto body
    )
    {
        var response = await _expenseService.UpdateExpenseRecord(expenseId, body);
        if (response.Success)
            return Ok(response);
        return NotFound(response);
    }

    [Authorize(Roles = "SuperAdmin")]
    [HttpDelete("{expenseId}")]
    public async Task<IActionResult> DeleteExpenseRecord([FromRoute] string expenseId)
    {
        var response = await _expenseService.DeleteExpenseRecord(expenseId);
        if (response.Success)
            return Ok(response);
        return BadRequest(response);
    }

    [Authorize]
    [HttpPost("upload/image")]
    [RequestSizeLimit(5 * 1024 * 1024)]
    public async Task<IActionResult> UploadImage(IFormFile file)
    {
        var response = await _expenseService.UploadImage(file);
        if (response.Success)
            return Ok(response);
        return BadRequest(response);
    }

    [Authorize]
    [HttpDelete("image")]
    public IActionResult DeleteImage([FromBody] DeleteExpenseImage body)
    {
        var response = _expenseService.DeleteImage(body.FilePath);
        if (response.Success)
            return Ok(response);
        return BadRequest(response);
    }
}
