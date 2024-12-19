using LinqToDB.Mapping;

namespace me.admin.api.Models;

[Table("tblDailyRecord")]
public class DailyRecord
{
	[PrimaryKey]
	public required string RecordId { get; set; }

	[Column][NotNull]
	public required string OutletId { get; set; }

	[Column][NotNull]
	public required long RecordDate { get; set; }

	[Column][NotNull]
	public required double Revenue { get; set; }

	[Column][NotNull]
	public required double Cash { get; set; }

	[Column][NotNull]
	public required string CreatedBy { get; set; }

	[Column][NotNull]
	public long? CreatedAt { get; set; }

	[Column][NotNull]
	public long? UpdatedAt { get; set; }

	[Column]
	public long? DeletedAt { get; set; }
}