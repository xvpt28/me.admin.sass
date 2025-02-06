using LinqToDB;
using me.admin.api.DTOs;
using me.admin.api.Models;

namespace me.admin.api.Data.Repositories;

public class OrderItemRepository(AppDbContext appDbContext)
{
    readonly AppDbContext _appDbContext = appDbContext;

    public async Task<List<OrderItem>> GetAll(string orderId)
    {
        await using var db = _appDbContext.GetDatabase();
        return await db.GetTable<OrderItem>()
            .Where(x => x.DeletedAt == null && x.OrderId == orderId)
            .OrderBy(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<OrderItemWithMenu>> GetAllWithMenu(string orderId)
    {
        await using var db = _appDbContext.GetDatabase();
        var tblOrderItem = db.GetTable<OrderItem>().Where(x => x.DeletedAt == null);
        var tblMenu = db.GetTable<Menu>().Where(x => x.DeletedAt == null);

        var query =
            from o in tblOrderItem
            join m in tblMenu on o.MenuId equals m.ItemId
            where o.OrderId == orderId
            select new OrderItemWithMenu
            {
                OrderItemId = o.OrderItemId,
                OrderId = o.OrderId,
                MenuId = o.MenuId,
                Quantity = o.Quantity,
                CreatedAt = o.CreatedAt,
                UpdatedAt = o.UpdatedAt,
                DeletedAt = o.DeletedAt,
                MenuName = m.Name,
                UnitPrice = o.UnitPrice,
            };

        return await query.ToListAsync();
    }

    public async Task<OrderItem?> GetById(string id)
    {
        await using var db = _appDbContext.GetDatabase();
        return await db.GetTable<OrderItem>()
            .Where(x => x.DeletedAt == null && x.OrderItemId == id)
            .FirstOrDefaultAsync();
    }

    public async Task Insert(OrderItem entity)
    {
        await using var db = _appDbContext.GetDatabase();
        await db.InsertAsync(entity);
    }

    public async Task Update(OrderItem entity)
    {
        await using var db = _appDbContext.GetDatabase();
        await db.UpdateAsync(entity);
    }

    public async Task Delete(string id)
    {
        await using var db = _appDbContext.GetDatabase();
        var response = await GetById(id);
        if (response == null)
            throw new Exception("Order not found");
        response.DeletedAt = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        await db.UpdateAsync(response);
    }
}
