namespace me.admin.api.DTOs;

public class CreateMenuDto
{
	public required string OutletId { get; set; }
	public required string Name { get; set; }
	public required double Price { get; set; }
	public required string Unit { get; set; }
}