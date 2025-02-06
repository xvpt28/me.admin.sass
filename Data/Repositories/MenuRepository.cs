using LinqToDB;
using me.admin.api.Interfaces;
using me.admin.api.Models;

namespace me.admin.api.Data.Repositories;

public class MenuRepository(AppDbContext appDbContext) : IRepositoryByKey<Menu>
{
    readonly AppDbContext _appDbContext = appDbContext;

    public async Task<List<Menu>> GetAll(string outletId)
    {
        await using var db = _appDbContext.GetDatabase();
        return await db.GetTable<Menu>()
            .Where(x => x.DeletedAt == null && x.OutletId == outletId)
            .ToListAsync();
    }

    public async Task<Menu?> GetById(string id)
    {
        await using var db = _appDbContext.GetDatabase();
        return await db.GetTable<Menu>()
            .Where(x => x.DeletedAt == null && x.ItemId == id)
            .FirstOrDefaultAsync();
    }

    public async Task Insert(Menu entity)
    {
        await using var db = _appDbContext.GetDatabase();
        await db.InsertAsync(entity);
    }

    public async Task Update(Menu entity)
    {
        await using var db = _appDbContext.GetDatabase();
        await db.UpdateAsync(entity);
    }

    public async Task Delete(string id)
    {
        await using var db = _appDbContext.GetDatabase();
        var user = await db.GetTable<Menu>().Where(x => x.ItemId == id).FirstOrDefaultAsync();
        if (user == null)
        {
            throw new Exception("User not found");
        }
        user.DeletedAt = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        await db.UpdateAsync(user);
    }
}
