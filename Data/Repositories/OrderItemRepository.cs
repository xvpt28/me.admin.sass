namespace me.admin.api.Data.Repositories;

public class OrderItemRepository(AppDbContext appDbContext)
{
	readonly AppDbContext _appDbContext = appDbContext;
}