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
				Price = request.Price,
				Unit = request.Unit,
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
}