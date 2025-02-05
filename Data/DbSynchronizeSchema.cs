using LinqToDB;

namespace me.admin.api.Data;

public class DbSynchronizeSchema(AppDbContext appDbContext)
{
	readonly AppDbContext _appDbContext = appDbContext;

	public void Synchronize()
	{
		// SynchronizeTable<User>();
		// SynchronizeTable<Outlet>();
		// SynchronizeTable<DailyRecord>();
		// SynchronizeTable<Menu>();
		// SynchronizeTable<Order>();
		// SynchronizeTable<OrderItem>();
		// SynchronizeTable<Importance>();
		// SynchronizeTable<Invoice>();
		// SynchronizeTable<Expense>();
	}

	void SynchronizeTable<T>() where T : class
	{
		try
		{
			var tableSchema = _appDbContext.GetDatabase().GetTable<T>();
			_appDbContext.GetDatabase().DropTable<T>();
			_appDbContext.GetDatabase().CreateTable<T>();
		}
		catch (Exception e)
		{
			_appDbContext.GetDatabase().CreateTable<T>();
		}
	}
}