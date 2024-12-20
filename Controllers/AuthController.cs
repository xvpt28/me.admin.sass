using me.admin.api.DTOs;
using me.admin.api.Services;
using Microsoft.AspNetCore.Mvc;

namespace me.admin.api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(AuthService authService) : ControllerBase
{
    readonly AuthService _authService = authService;

    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequest)
    {
        var response = await _authService.Login(loginRequest);

        if (response.Success)
            return Ok(response);
        return Unauthorized(response);
    }
}
