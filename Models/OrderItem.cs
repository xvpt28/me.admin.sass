using LinqToDB.Mapping;

namespace me.admin.api.Models;

[Table("tblOrderItem")]
public class OrderItem
{
	[PrimaryKey]
	public required string OrderItemId { get; set; }

	[Column][NotNull]
	public required string OrderId { get; set; }

	[Column][NotNull]
	public required string MenuId { get; set; }

	[Column][NotNull]
	public required int Quantity { get; set; }
}