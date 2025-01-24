namespace me.admin.api.DTOs;

public class CreateOrderItemDto
{
	public required string MenuId { get; set; }
	public required float UnitPrice { get; set; }
}

public class UpdateOrderItemDto
{
	public required int Quantity { get; set; }
}

public class OrderItemWithMenu
{
	public required string OrderItemId { get; set; }
	public required string OrderId { get; set; }
	public required int Quantity { get; set; }
	public required double UnitPrice { get; set; }
	public required string MenuId { get; set; }
	public required string MenuName { get; set; }
	public long? DeletedAt { get; set; }
	public long? CreatedAt { get; set; }
	public long? UpdatedAt { get; set; }
}