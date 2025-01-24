using LinqToDB.Mapping;

namespace me.admin.api.Models;

[Table("tblMenu")]
public class Menu
{
	[PrimaryKey]
	public required string ItemId { get; set; }

	[Column][NotNull]
	public required string OutletId { get; set; }

	[Column][NotNull]
	public required string Name { get; set; }

	[Column][NotNull]
	public required double Price { get; set; }

	[Column]
	public long? DeletedAt { get; set; }

	[Column][NotNull]
	public long? CreatedAt { get; set; }

	[Column][NotNull]
	public long? UpdatedAt { get; set; }
}