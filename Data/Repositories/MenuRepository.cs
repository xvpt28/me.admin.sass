namespace me.admin.api.Data.Repositories;

public class MenuRepository(AppDbContext appDbContext)
{
	readonly AppDbContext _appDbContext = appDbContext;
}