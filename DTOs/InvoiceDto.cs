namespace me.admin.api.DTOs;

public class CreateInvoiceDto
{
	public required string Type { get; set; }
	public string? BilledTo { get; set; }
	public string? BilledCompanyAddress { get; set; }
	public string? BilledCompanyUEN { get; set; }

	public bool HideDiscount { get; set; } = false;
}