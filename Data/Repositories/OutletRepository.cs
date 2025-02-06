using LinqToDB;
using me.admin.api.Interfaces;
using me.admin.api.Models;

namespace me.admin.api.Data.Repositories;

public class OutletRepository(AppDbContext appDbContext) : IRepository<Outlet>
{
    readonly AppDbContext _appDbContext = appDbContext;

    public async Task<List<Outlet>> GetAll()
    {
        await using var db = _appDbContext.GetDatabase();
        return await db.GetTable<Outlet>().Where(x => x.DeletedAt == null).ToListAsync();
    }

    public async Task<Outlet?> GetById(string outletId)
    {
        await using var db = _appDbContext.GetDatabase();
        return await db.GetTable<Outlet>()
            .Where(x => x.OutletId == outletId && x.DeletedAt == null)
            .FirstOrDefaultAsync();
    }

    public async Task Insert(Outlet entity)
    {
        await using var db = _appDbContext.GetDatabase();
        await db.InsertAsync(entity);
    }

    public async Task Update(Outlet entity)
    {
        await using var db = _appDbContext.GetDatabase();
        await db.UpdateAsync(entity);
    }

    public async Task Delete(string id)
    {
        await using var db = _appDbContext.GetDatabase();
        var outlet = await db.GetTable<Outlet>().Where(x => x.OutletId == id).FirstOrDefaultAsync();
        if (outlet == null)
        {
            throw new Exception("User not found");
        }
        outlet.DeletedAt = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        await db.UpdateAsync(outlet);
    }

    public async Task<Outlet?> GetByOutletName(string outletName)
    {
        await using var db = _appDbContext.GetDatabase();
        return await db.GetTable<Outlet>()
            .Where(x => x.OutletName == outletName && x.DeletedAt == null)
            .FirstOrDefaultAsync();
    }
}
