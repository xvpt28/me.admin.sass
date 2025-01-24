using LinqToDB;
using me.admin.api.DTOs;
using me.admin.api.Models;

namespace me.admin.api.Data.Repositories;

public class DailyRecordRepository(AppDbContext appDbContext)
{
	readonly AppDbContext _appDbContext = appDbContext;

	public async Task<List<GetDailyRecordResponseDto>> GetAll(string outletId)
	{
		await using var db = _appDbContext.GetDatabase();
		var tblDr = db.GetTable<DailyRecord>().Where(x => x.DeletedAt == null && x.OutletId == outletId);
		var tblUser = db.GetTable<User>();
		var tblOutlet = db.GetTable<Outlet>();
		var query = from dr in tblDr
			join user in tblUser on dr.CreatedBy equals user.UserId
			join outlet in tblOutlet on dr.OutletId equals outlet.OutletId
			select new GetDailyRecordResponseDto
			{
				RecordId = dr.RecordId,
				OutletId = dr.OutletId,
				RecordDate = dr.RecordDate,
				Revenue = dr.Revenue,
				Cash = dr.Cash,
				CreatedBy = dr.CreatedBy,
				CreatedAt = dr.CreatedAt,
				UpdatedAt = dr.UpdatedAt,
				OutletInfo = outlet,
				UserInfo = new GetUserDto
				{
					UserId = user.UserId,
					Email = user.Email,
					FullName = user.FullName,
					Role = user.Role
				}
			};
		return await query.ToListAsync();
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

	public async Task<DailyRecord?> GetById(string id)
	{
		await using var db = _appDbContext.GetDatabase();
		return await db.GetTable<DailyRecord>().Where(x => x.DeletedAt == null && x.RecordId == id).FirstOrDefaultAsync();
	}

	public async Task Insert(DailyRecord entity)
	{
		await using var db = _appDbContext.GetDatabase();
		await db.InsertAsync(entity);
	}

	public async Task Update(DailyRecord entity)
	{
		await using var db = _appDbContext.GetDatabase();
		await db.UpdateAsync(entity);
	}

	public async Task<List<GetDailyRecordResponseDto>> GetAllByDate(long date)
	{
		await using var db = _appDbContext.GetDatabase();
		var tblDr = db.GetTable<DailyRecord>().Where(x => x.RecordDate == date && x.DeletedAt == null);
		var tblUser = db.GetTable<User>();
		var tblOutlet = db.GetTable<Outlet>();
		var query = from dr in tblDr
			join user in tblUser on dr.CreatedBy equals user.UserId
			join outlet in tblOutlet on dr.OutletId equals outlet.OutletId
			select new GetDailyRecordResponseDto
			{
				RecordId = dr.RecordId,
				OutletId = dr.OutletId,
				RecordDate = dr.RecordDate,
				Revenue = dr.Revenue,
				Cash = dr.Cash,
				CreatedBy = dr.CreatedBy,
				CreatedAt = dr.CreatedAt,
				UpdatedAt = dr.UpdatedAt,
				OutletInfo = outlet,
				UserInfo = new GetUserDto
				{
					UserId = user.UserId,
					Email = user.Email,
					FullName = user.FullName,
					Role = user.Role
				}
			};
		return await query.ToListAsync();
	}

	public async Task<DailyRecord?> GetByDateAndOutletId(string outletId, long date)
	{
		await using var db = _appDbContext.GetDatabase();
		return await db.GetTable<DailyRecord>().Where(x => x.DeletedAt == null && x.OutletId == outletId && x.RecordDate == date).FirstOrDefaultAsync();
	}
}