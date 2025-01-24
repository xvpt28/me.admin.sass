namespace me.admin.api.DTOs;

public class CreateOrderDto
{
	public required string OrderStatus { get; set; }
	public required string Remarks { get; set; }
}

public class UpdateOrderDto
{
	public float? Discount { get; set; }
	public double? Amount { get; set; }
	public string? OrderStatus { get; set; }
	public string? Remarks { get; set; }

	public string? PaymentMethod { get; set; }
}

public class GetOrderFilterDto
{
	public required int Page { get; set; }
	public required int Limit { get; set; }
	public string? OrderStatus { get; set; }
	public long? StartTimestamp { get; set; }
	public long? EndTimestamp { get; set; }
}