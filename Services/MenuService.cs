using me.admin.api.Data.Repositories;
using me.admin.api.DTOs;
using me.admin.api.Models;
using Serilog;

namespace me.admin.api.Services;

public class MenuService(MenuRepository menuRepository)
{
	readonly MenuRepository _menuRepository = menuRepository;

	public async Task<BaseResponse<string>> CreateMenu(CreateMenuDto request)
	{
		try
		{
			var id = Ulid.NewUlid().ToString();
			var currentTimestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
			var entity = new Menu
			{
				ItemId = id,
				OutletId = request.OutletId,
				Name = request.Name,
				Price = Math.Round(request.Price, 2),
				CreatedAt = currentTimestamp,
				UpdatedAt = currentTimestamp
			};

			await _menuRepository.Insert(entity);
			return new BaseResponse<string>(id) { Success = true };
		}
		catch (Exception e)
		{
			Log.Logger.Error(e.Message, "Error creating menu");
			return new BaseResponse<string> { Success = false, Message = e.Message };
		}
	}

	public async Task<BaseResponse<List<Menu>>> GetAllMenuByOutlet(string outletId)
	{
		var menus = await _menuRepository.GetAll(outletId);
		return new BaseResponse<List<Menu>>(menus) { Success = true };
	}

	public async Task<BaseResponse<bool>> UpdateMenuById(string id, UpdateMenuDto request)
	{
		try
		{
			var menu = await _menuRepository.GetById(id);
			if (menu == null) return new BaseResponse<bool> { Success = false, Message = "Menu not found" };
			menu.Name = request.Name ?? menu.Name;
			menu.Price = Math.Round(request.Price ?? menu.Price, 2);
			await _menuRepository.Update(menu);
			return new BaseResponse<bool>(true) { Success = true };
		}
		catch (Exception e)
		{
			Log.Logger.Error(e.Message, "Error updating menu");
			return new BaseResponse<bool> { Success = false, Message = e.Message };
		}
	}

	public async Task<BaseResponse<bool>> DeleteMenuById(string id)
	{
		await _menuRepository.Delete(id);
		return new BaseResponse<bool> { Success = true };
	}
}