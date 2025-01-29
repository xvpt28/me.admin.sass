using LinqToDB;
using me.admin.api.DTOs;
using me.admin.api.Models;

namespace me.admin.api.Data.Repositories;

public class OrderRepository(AppDbContext appDbContext)
{
	readonly AppDbContext _appDbContext = appDbContext;

	public async Task Delete(string id)
	{
		await using var db = _appDbContext.GetDatabase();
		var response = await GetById(id);
		if (response == null)
			throw new Exception("Order not found");
		response.DeletedAt = DateTimeOffset.Now.ToUnixTimeSeconds();
		await db.UpdateAsync(response);
	}

	public async Task<List<Order>> GetAllOngoingOrder(string outletId)
	{
		await using var db = _appDbContext.GetDatabase();
		var tblOrder = db.GetTable<Order>()
			.Where(x => x.DeletedAt == null && x.OutletId == outletId && x.OrderStatus == "ongoing");
		var tblUser = db.GetTable<User>().Where(x => x.DeletedAt == null);

		var query =
			from o in tblOrder
			join u in tblUser on o.CreatedBy equals u.UserId into ou
			from u in ou.DefaultIfEmpty()
			group o by new { o, u.FullName } into orderGroup
			orderby orderGroup.Key.o.CreatedAt
			select new Order
			{
				OrderId = orderGroup.Key.o.OrderId,
				OutletId = orderGroup.Key.o.OutletId,
				OrderStatus = orderGroup.Key.o.OrderStatus,
				Remarks = orderGroup.Key.o.Remarks,
				Discount = orderGroup.Key.o.Discount,
				Amount = orderGroup.Key.o.Amount,
				PaymentMethod = orderGroup.Key.o.PaymentMethod,
				CreatedAt = orderGroup.Key.o.CreatedAt,
				UpdatedAt = orderGroup.Key.o.UpdatedAt,
				DeletedAt = orderGroup.Key.o.DeletedAt,
				CreatedBy = orderGroup.Key.FullName
			};
		return await query.ToListAsync();
	}

	public async Task<List<Order>> GetAllOrdersWithFilters(string outletId, GetOrderFilterDto filter)
	{
		var page = filter.Page;
		var limit = filter.Limit;
		var skip = limit * (page - 1);
		await using var db = _appDbContext.GetDatabase();
		var tblOrder = db.GetTable<Order>()
			.Where(x => x.DeletedAt == null && x.OutletId == outletId);

		if (filter.OrderStatus != null)
		{
			tblOrder = tblOrder.Where(x => x.OrderStatus == filter.OrderStatus);
		}

		var tblUser = db.GetTable<User>().Where(x => x.DeletedAt == null);
		var query =
			from o in tblOrder
			join u in tblUser on o.CreatedBy equals u.UserId into ou
			from u in ou.DefaultIfEmpty()
			group o by new { o, u.FullName } into orderGroup
			orderby orderGroup.Key.o.CreatedAt
			select new Order
			{
				OrderId = orderGroup.Key.o.OrderId,
				OutletId = orderGroup.Key.o.OutletId,
				OrderStatus = orderGroup.Key.o.OrderStatus,
				Remarks = orderGroup.Key.o.Remarks,
				Discount = orderGroup.Key.o.Discount,
				Amount = orderGroup.Key.o.Amount,
				PaymentMethod = orderGroup.Key.o.PaymentMethod,
				CreatedAt = orderGroup.Key.o.CreatedAt,
				UpdatedAt = orderGroup.Key.o.UpdatedAt,
				DeletedAt = orderGroup.Key.o.DeletedAt,
				CreatedBy = orderGroup.Key.FullName
			};

		if (filter.StartTimestamp != null && filter.EndTimestamp != null)
		{
			query = query.Where(x => x.CreatedAt >= filter.StartTimestamp && x.CreatedAt <= filter.EndTimestamp);
		}

		if (filter.OrderStatus != null)
		{
			query = query.Where(x => x.OrderStatus == filter.OrderStatus);
		}

		query = query.Skip(skip).Take(limit);

		return await query.ToListAsync();
	}

	public async Task<List<Order>> GetAllOrderPayByCashByDate(string outletId, long startTimestamp, long endTimestamp)
	{
		await using var db = _appDbContext.GetDatabase();
		return await db.GetTable<Order>()
			.Where(x => x.DeletedAt == null && x.OutletId == outletId && x.PaymentMethod == "cash" && x.CreatedAt >= startTimestamp && x.CreatedAt <= endTimestamp)
			.ToListAsync();
	}

	public async Task<Order?> GetById(string id)
	{
		await using var db = _appDbContext.GetDatabase();
		return await db.GetTable<Order>()
			.Where(x => x.DeletedAt == null && x.OrderId == id)
			.FirstOrDefaultAsync();
	}

	public async Task Insert(Order entity)
	{
		await using var db = _appDbContext.GetDatabase();
		await db.InsertAsync(entity);
	}

	public async Task Update(Order entity)
	{
		await using var db = _appDbContext.GetDatabase();
		await db.UpdateAsync(entity);
	}
}