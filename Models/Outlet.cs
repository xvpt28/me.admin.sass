using LinqToDB.Mapping;

namespace me.admin.api.Models;

[Table("tblOutlet")]
public class Outlet
{
	[PrimaryKey]
	public required string OutletId { get; set; }

	[Column][NotNull]
	public required string OutletName { get; set; }

	[Column]
	public required string OutletAddress { get; set; }

	[Column]
	public required string OutletPostcode { get; set; }

	[Column]
	public required string OutletPhoneNumber { get; set; }

	[Column]
	public required string OutletStatus { get; set; }

	[Column]
	public long? DeletedAt { get; set; }

	[Column][NotNull]
	public long? CreatedAt { get; set; }

	[Column][NotNull]
	public long? UpdatedAt { get; set; }
}