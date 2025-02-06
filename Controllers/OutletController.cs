using me.admin.api.DTOs;
using me.admin.api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace me.admin.api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OutletController(OutletService outletService) : ControllerBase
{
    readonly OutletService _outletService = outletService;

    [HttpGet("{outletId}")]
    public async Task<IActionResult> GetOutletById([FromRoute] string outletId)
    {
        var response = await _outletService.GetOutletById(outletId);

        if (response.Success)
            return Ok(response);
        return Unauthorized(response);
    }

    [Authorize]
    [HttpGet("all")]
    public async Task<IActionResult> GetAllOutlets()
    {
        var response = await _outletService.GetAllOutlets();

        if (response.Success)
            return Ok(response);
        return Unauthorized(response);
    }

    [Authorize(Roles = "SuperAdmin")]
    [HttpPost("create")]
    public async Task<IActionResult> CreateOutlet([FromBody] CreateOutletRequestDto body)
    {
        var response = await _outletService.CreateOutlet(body);

        if (response.Success)
            return Ok(response);
        return Unauthorized(response);
    }

    [Authorize(Roles = "SuperAdmin")]
    [HttpPut("update/{outletId}")]
    public async Task<IActionResult> UpdateOutlet(
        [FromRoute] string outletId,
        [FromBody] UpdateOutletRequestDto body
    )
    {
        var response = await _outletService.UpdateOutlet(outletId, body);

        if (response.Success)
            return Ok(response);
        return Unauthorized(response);
    }

    [Authorize(Roles = "SuperAdmin")]
    [HttpDelete("{outletId}")]
    public async Task<IActionResult> DeleteOutlet([FromRoute] string outletId)
    {
        var response = await _outletService.DeleteOutlet(outletId);

        if (response.Success)
            return Ok(response);
        return Unauthorized(response);
    }
}
