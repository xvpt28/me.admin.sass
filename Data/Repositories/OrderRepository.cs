namespace me.admin.api.Data.Repositories;

public class OrderRepository(AppDbContext appDbContext)
{
	readonly AppDbContext _appDbContext = appDbContext;
}