using LinqToDB;
using me.admin.api.Models;

namespace me.admin.api.Data.Repositories;

public class InvoiceRepository(AppDbContext appDbContext)
{
	readonly AppDbContext _appDbContext = appDbContext;

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