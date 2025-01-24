using LinqToDB.Mapping;

namespace me.admin.api.Models;

[Table("tblInvoice")]
public class Invoice
{
	[PrimaryKey][Identity]
	public int InvoiceId { get; set; }

	[Column][NotNull]
	public required string OrderId { get; set; }

	[Column][NotNull]
	public required string Type { get; set; }

	[Column][Nullable]
	public string? FilePath { get; set; }

	[Column][Nullable]
	public string? BilledTo { get; set; }

	[Column][Nullable]
	public string? BilledCompanyAddress { get; set; }

	[Column][Nullable]
	public string? BilledCompanyUEN { get; set; }

	[Column][NotNull]
	public required long IssuedDate { get; set; }

	[Column][NotNull]
	public required string CreatedBy { get; set; }

	[Column][NotNull]
	public long? CreatedAt { get; set; }

	[Column][NotNull]
	public long? UpdatedAt { get; set; }

	[Column]
	public long? DeletedAt { get; set; }
}