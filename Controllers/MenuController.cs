using me.admin.api.DTOs;
using me.admin.api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace me.admin.api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MenuController(MenuService menuService) : ControllerBase
{
    readonly MenuService _menuService = menuService;

    [Authorize]
    [HttpPost("create")]
    public async Task<IActionResult> CreateMenu([FromBody] CreateMenuDto body)
    {
        var response = await _menuService.CreateMenu(body);
        if (response.Success)
            return Ok(response);
        return Unauthorized(response);
    }

    [Authorize]
    [HttpGet("all/{outletId}")]
    public async Task<IActionResult> GetAllMenu([FromRoute] string outletId)
    {
        var response = await _menuService.GetAllMenuByOutlet(outletId);
        if (response.Success)
            return Ok(response);
        return Unauthorized(response);
    }

    [Authorize]
    [HttpPut("update/{menuId}")]
    public async Task<IActionResult> UpdateMenuById(
        [FromRoute] string menuId,
        [FromBody] UpdateMenuDto body
    )
    {
        var response = await _menuService.UpdateMenuById(menuId, body);
        if (response.Success)
            return Ok(response);
        return Unauthorized(response);
    }

    [Authorize]
    [HttpDelete("{menuId}")]
    public async Task<IActionResult> DeleteMenuById([FromRoute] string menuId)
    {
        var response = await _menuService.DeleteMenuById(menuId);
        if (response.Success)
            return Ok(response);
        return Unauthorized(response);
    }
}
