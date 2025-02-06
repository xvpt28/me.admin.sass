namespace me.admin.api.Utils;

public class FileSetting
{
	public required string RootFolder { get; init; }
	public required string InvoiceRootFolder { get; init; }
	public required string ExpenseRootFolder { get; init; }
	public required string ProcurementRootFolder { get; init; }
}