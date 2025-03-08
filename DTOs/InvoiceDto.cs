using me.admin.api.Models;

namespace me.admin.api.DTOs;

public class CreateInvoiceDto
{
    public required string Type { get; set; }
    public string? PoNumber { get; set; }
    public string? BilledTo { get; set; }
    public string? BilledCompanyAddress { get; set; }
    public string? BilledCompanyUEN { get; set; }
    public string? BilledToEmail { get; set; }
    public bool PocRequired { get; set; } = false;

    public bool HideDiscount { get; set; } = false;

    public double? Discount { get; set; }

    public double? Amount { get; set; }
}

public class GetInvoiceFilterDto
{
    public string? Search { get; set; }
    public required int Page { get; set; }
    public required int Limit { get; set; }
    public required string Type { get; set; }
    public long? StartDate { get; set; }
    public long? EndDate { get; set; }
}

public class GetAllInvoiceResponseDto : Invoice
{
    public required long OrderedAt { get; set; }
}
