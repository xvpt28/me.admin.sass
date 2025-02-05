using LinqToDB.Mapping;

namespace me.admin.api.Models;

[Table("tblExpense")]
public class Expense
{
	[PrimaryKey]
	public required string ExpenseId { get; set; }

	[Column][NotNull]
	public required string OutletId { get; set; }

	[Column][NotNull]
	public required long ClaimDate { get; set; }

	[Column][NotNull]
	public required double Amount { get; set; }

	[Column]
	public string? Remark { get; set; }

	[Column][NotNull]
	public required string Status { get; set; }

	[Column][NotNull]
	public required string Type { get; set; }

	[Column]
	public string? FilePaths { get; set; }

	[Column][NotNull]
	public required bool IsClaimRequired { get; set; }

	[Column][NotNull]
	public required string ClaimBy { get; set; }

	[Column]
	public string? UpdatedBy { get; set; }

	[Column][NotNull]
	public long? CreatedAt { get; set; }

	[Column][NotNull]
	public long? UpdatedAt { get; set; }

	[Column]
	public long? DeletedAt { get; set; }
}