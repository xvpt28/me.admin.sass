using me.admin.api.DTOs;
using me.admin.api.Services;
using Microsoft.AspNetCore.Mvc;

namespace me.admin.api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MenuController(MenuService menuService) : ControllerBase
{
	readonly MenuService _menuService = menuService;

	[HttpPost("new")]
	public async Task<IActionResult> CreateMenu([FromRoute] string outletId, [FromBody] CreateMenuDto body)
	{
		var response = await _menuService.CreateMenu(body);
		if (response.Success)
			return Ok(response);
		return Unauthorized(response);
	}
}