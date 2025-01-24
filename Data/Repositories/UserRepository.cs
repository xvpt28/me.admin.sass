using LinqToDB;
using me.admin.api.Interfaces;
using me.admin.api.Models;

namespace me.admin.api.Data.Repositories;

public class UserRepository(AppDbContext appDbContext) : IRepository<User>
{
	readonly AppDbContext _appDbContext = appDbContext;

	public async Task<List<User>> GetAll()
	{
		await using var db = _appDbContext.GetDatabase();
		return await db.GetTable<User>().Where(x => x.DeletedAt == null && x.IsActive == true).ToListAsync();
	}

	public async Task<User?> GetById(string userId)
	{
		await using var db = _appDbContext.GetDatabase();
		return await db.GetTable<User>().Where(x => x.UserId == userId && x.DeletedAt == null).FirstOrDefaultAsync();
	}

	public async Task Insert(User entity)
	{
		await using var db = _appDbContext.GetDatabase();
		await db.InsertAsync(entity);
	}

	public async Task Update(User entity)
	{
		await using var db = _appDbContext.GetDatabase();
		await db.UpdateAsync(entity);
	}

	public async Task Delete(string id)
	{
		await using var db = _appDbContext.GetDatabase();
		var user = await db.GetTable<User>().Where(x => x.UserId == id).FirstOrDefaultAsync();
		if (user == null)
		{
			throw new Exception("User not found");
		}
		user.DeletedAt = DateTimeOffset.Now.ToUnixTimeMilliseconds();
		await db.UpdateAsync(user);
	}

	public async Task UpdateLastLogin(string userId)
	{
		await using var db = _appDbContext.GetDatabase();
		var user = await db.GetTable<User>().Where(x => x.UserId == userId && x.DeletedAt == null).FirstOrDefaultAsync();
		if (user == null)
		{
			throw new Exception("User not found");
		}
		user.LastLogin = DateTimeOffset.Now.ToUnixTimeMilliseconds();
		await db.UpdateAsync(user);
	}

	public async Task<User?> GetUserByEmail(string email)
	{
		await using var db = _appDbContext.GetDatabase();
		return await db.GetTable<User>().Where(x => x.Email == email && x.DeletedAt == null).FirstOrDefaultAsync();
	}
}