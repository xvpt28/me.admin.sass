using LinqToDB;
using me.admin.api.Models;

namespace me.admin.api.Data.Repositories;

public class ImportanceRepository(AppDbContext appDbContext)
{
    readonly AppDbContext _appDbContext = appDbContext;

    public async Task<List<Importance>> GetAll(string outletId)
    {
        await using var db = _appDbContext.GetDatabase();
        var tblImp = db.GetTable<Importance>()
            .Where(x => x.DeletedAt == null && x.OutletId == outletId && x.IsArchived == false)
            .OrderBy(x => x.CreatedAt);
        var tblUser = db.GetTable<User>();
        var query =
            from imp in tblImp
            join user in tblUser on imp.CreatedBy equals user.UserId
            select new Importance
            {
                NoteId = imp.NoteId,
                OutletId = imp.OutletId,
                Title = imp.Title,
                Content = imp.Content,
                Type = imp.Type,
                IsArchived = imp.IsArchived,
                CreatedBy = user.FullName,
                CreatedAt = imp.CreatedAt,
                UpdatedAt = imp.UpdatedAt,
            };
        return await query.ToListAsync();
    }

    public async Task<Importance?> GetById(string id)
    {
        await using var db = _appDbContext.GetDatabase();
        return await db.GetTable<Importance>()
            .Where(x => x.DeletedAt == null && x.NoteId == id)
            .FirstOrDefaultAsync();
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

    public async Task Insert(Importance entity)
    {
        await using var db = _appDbContext.GetDatabase();
        await db.InsertAsync(entity);
    }

    public async Task Update(Importance entity)
    {
        await using var db = _appDbContext.GetDatabase();
        await db.UpdateAsync(entity);
    }
}
