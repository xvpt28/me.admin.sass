using LinqToDB;
using me.admin.api.Models;

namespace me.admin.api.Data.Repositories;

public class ExpenseRepository(AppDbContext appDbContext)
{
    readonly AppDbContext _appDbContext = appDbContext;

    public async Task<List<Expense>> GetAllByOutletAndDate(
        string outletId,
        long startDate,
        long endDate,
        bool isClaimRequired,
        string method
    )
    {
        await using var db = _appDbContext.GetDatabase();
        return await db.GetTable<Expense>()
            .Where(x =>
                x.DeletedAt == null
                && x.OutletId == outletId
                && x.ClaimDate >= startDate
                && x.ClaimDate <= endDate
                && x.IsClaimRequired == isClaimRequired
                && x.Method == method
            )
            .ToListAsync();
    }

    public async Task<Expense?> GetById(string id)
    {
        await using var db = _appDbContext.GetDatabase();
        return await db.GetTable<Expense>()
            .Where(x => x.DeletedAt == null && x.ExpenseId == id)
            .FirstOrDefaultAsync();
    }

    public async Task Insert(Expense entity)
    {
        await using var db = _appDbContext.GetDatabase();
        await db.InsertAsync(entity);
    }

    public async Task Update(Expense entity)
    {
        await using var db = _appDbContext.GetDatabase();
        await db.UpdateAsync(entity);
    }

    public async Task Delete(string id)
    {
        await using var db = _appDbContext.GetDatabase();
        var response = await GetById(id);
        if (response == null)
        {
            throw new Exception("User not found");
        }
        response.DeletedAt = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        await db.UpdateAsync(response);
    }
}
