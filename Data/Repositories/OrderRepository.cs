using LinqToDB;
using me.admin.api.Interfaces;
using me.admin.api.Models;

namespace me.admin.api.Data.Repositories;

public class OrderRepository(AppDbContext appDbContext) : IRepositoryByKey<Order>
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

    public async Task<List<Order>> GetAll(string outletId)
    {
        await using var db = _appDbContext.GetDatabase();
        return await db.GetTable<Order>()
            .Where(x => x.DeletedAt == null && x.OutletId == outletId)
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
