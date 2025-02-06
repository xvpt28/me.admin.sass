using me.admin.api.DTOs;
using me.admin.api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace me.admin.api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProcurementController(ProcurementService procurementService) : ControllerBase
{
    readonly ProcurementService _procurementService = procurementService;

    [Authorize]
    [HttpPost("upload/image")]
    [RequestSizeLimit(5 * 1024 * 1024)]
    public async Task<IActionResult> UploadImage(IFormFile file)
    {
        var response = await _procurementService.UploadImage(file);
        if (response.Success)
            return Ok(response);
        return BadRequest(response);
    }
}
