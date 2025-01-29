using LinqToDB;
using me.admin.api.DTOs;
using me.admin.api.Models;

namespace me.admin.api.Data.Repositories;

public class InvoiceRepository(AppDbContext appDbContext)
{
	readonly AppDbContext _appDbContext = appDbContext;

	public async Task<List<GetAllInvoiceResponseDto>> GetAllWithFilter(string outletId, GetInvoiceFilterDto filter)
	{
		await using var db = _appDbContext.GetDatabase();
		var skip = (filter.Page - 1) * filter.Limit;
		var tblInvoice = db.GetTable<Invoice>().Where(x => x.DeletedAt == null);
		var tblOrder = db.GetTable<Order>();

		var query = from inv in tblInvoice
			join o in tblOrder on inv.OrderId equals o.OrderId
			where o.OutletId == outletId
			where inv.Type == filter.Type
			select new GetAllInvoiceResponseDto
			{
				InvoiceId = inv.InvoiceId,
				OrderId = inv.OrderId,
				Type = inv.Type,
				FilePath = inv.FilePath,
				BilledTo = inv.BilledTo,
				BilledCompanyAddress = inv.BilledCompanyAddress,
				BilledCompanyUEN = inv.BilledCompanyUEN,
				IssuedDate = inv.IssuedDate,
				CreatedAt = inv.CreatedAt,
				CreatedBy = inv.CreatedBy,
				UpdatedAt = inv.UpdatedAt,
				DeletedAt = inv.DeletedAt,
				OrderedAt = o.CreatedAt
			};

		if (filter.StartDate != null && filter.EndDate != null)
		{
			query = from i in query where i.CreatedAt >= filter.StartDate && i.CreatedAt <= filter.EndDate select i;
		}

		query = query.Skip(skip).Take(filter.Limit).OrderBy(x => x.CreatedAt);

		if (filter.Search != null && filter.Search.Trim() != string.Empty)
		{
			var search = filter.Search.Trim();
			query = query.Where(x => x.InvoiceId.ToString().Trim().Contains(search)
				|| (x.BilledTo ?? "").Trim().Contains(search)
				|| (x.BilledCompanyAddress ?? "").Trim().Contains(search)
				|| (x.BilledCompanyUEN ?? "").Trim().Contains(search)
				|| (x.FilePath ?? "").Trim().Contains(search));
		}

		return await query.ToListAsync();
	}

	public async Task<List<Invoice>> GetAll(string outletId)
	{
		await using var db = _appDbContext.GetDatabase();
		var tblInvoice = db.GetTable<Invoice>().Where(x => x.DeletedAt == null);
		var tblOrder = db.GetTable<Order>().Where(x => x.DeletedAt == null);

		var query = from inv in tblInvoice
			join o in tblOrder on inv.OrderId equals o.OrderId
			where o.OutletId == outletId
			select inv;

		return await query.ToListAsync();
	}

	public async Task<Invoice?> GetById(int invoiceId)
	{
		await using var db = _appDbContext.GetDatabase();
		return await db.GetTable<Invoice>().Where(x => x.DeletedAt == null && x.InvoiceId == invoiceId).FirstOrDefaultAsync();
	}

	public async Task<Invoice?> GetByOrderId(string orderId)
	{
		await using var db = _appDbContext.GetDatabase();
		var tblInvoice = db.GetTable<Invoice>().Where(x => x.DeletedAt == null);
		var tblOrder = db.GetTable<Order>().Where(x => x.DeletedAt == null);

		var query = from inv in tblInvoice
			join o in tblOrder on inv.OrderId equals o.OrderId
			where o.OrderId == orderId
			select inv;

		return await query.FirstOrDefaultAsync();
	}

	public async Task Insert(Invoice entity)
	{
		await using var db = _appDbContext.GetDatabase();
		await db.InsertAsync(entity);
	}

	public async Task Update(Invoice entity)
	{
		await using var db = _appDbContext.GetDatabase();
		await db.UpdateAsync(entity);
	}

	public async Task Delete(int invoiceId)
	{
		await using var db = _appDbContext.GetDatabase();
		var invoice = await db.GetTable<Invoice>().Where(x => x.InvoiceId == invoiceId).FirstOrDefaultAsync();
		if (invoice == null) throw new Exception("Invoice not found");
		invoice.DeletedAt = DateTimeOffset.Now.ToUnixTimeMilliseconds();
		await db.UpdateAsync(invoice);
	}
}