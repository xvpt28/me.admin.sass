using me.admin.api.DTOs;
using me.admin.api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace me.admin.api.Controllers;

[Route("api/admin/[controller]")]
[ApiController]
public class UserController(UserService userService, AuthService authService) : ControllerBase
{
    readonly AuthService _authService = authService;
    readonly UserService _userService = userService;

    [HttpPost("initialization")]
    public async Task<IActionResult> Initialization()
    {
        var response = await _userService.Initialization();
        if (response.Success)
        {
            return Ok(response);
        }
        return BadRequest(response);
    }

    [Authorize(Roles = "SuperAdmin")]
    [HttpPost("create")]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserDto request)
    {
        var response = await _userService.CreateUser(request);
        if (response.Success)
        {
            return Ok(response);
        }
        return BadRequest(response);
    }

    [Authorize(Roles = "SuperAdmin")]
    [HttpPut("update/{userId}")]
    public async Task<IActionResult> UpdateUser(
        [FromRoute] string userId,
        [FromBody] UpdateUserDto request
    )
    {
        var response = await _userService.UpdateUser(userId, request);
        if (response.Success)
        {
            return Ok(response);
        }
        return BadRequest(response);
    }

    [Authorize]
    [HttpPut("update/password")]
    public async Task<IActionResult> UpdateMyPassword([FromBody] UpdateUserPasswordDto request)
    {
        var userId = _authService.GetUserId();
        if (userId == null)
            throw new Exception("Invalid token");
        var response = await _userService.UpdateMyPassword(userId, request);
        if (response.Success)
        {
            return Ok(response);
        }
        return BadRequest(response);
    }

    [Authorize(Roles = "SuperAdmin")]
    [HttpGet("all")]
    public async Task<IActionResult> GetAllUsers()
    {
        var response = await _userService.GetAllUsers();
        if (response.Success)
        {
            return Ok(response);
        }
        return BadRequest(response);
    }

    [Authorize(Roles = "SuperAdmin")]
    [HttpDelete("{userId}")]
    public async Task<IActionResult> DeleteUser([FromRoute] string userId)
    {
        var response = await _userService.DeleteUser(userId);
        if (response.Success)
        {
            return Ok(response);
        }
        return BadRequest(response);
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetMe()
    {
        var userId = _authService.GetUserId();
        if (userId == null)
        {
            return BadRequest("Invalid token");
        }
        var response = await _userService.GetUserById(userId);
        return Ok(response);
    }
}
