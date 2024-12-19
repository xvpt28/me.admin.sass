using me.admin.api.Data.Repositories;

namespace me.admin.api.Services;

public class MenuService(MenuRepository menuRepository)
{
	readonly MenuRepository _menuRepository = menuRepository;
}