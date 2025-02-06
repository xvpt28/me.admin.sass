using LinqToDB.Mapping;

namespace me.admin.api.Models;

[Table("tblImportance")]
public class Importance
{
    [PrimaryKey]
    public required string NoteId { get; set; }

    [Column]
    [NotNull]
    public required string OutletId { get; set; }

    [Column]
    [NotNull]
    public required string Title { get; set; }

    [Column]
    [NotNull]
    public required string Content { get; set; }

    [Column]
    [NotNull]
    public required string Type { get; set; }

    [Column]
    [NotNull]
    public required bool IsArchived { get; set; }

    [Column]
    [NotNull]
    public required string CreatedBy { get; set; }

    [Column]
    [NotNull]
    public long? CreatedAt { get; set; }

    [Column]
    [NotNull]
    public long? UpdatedAt { get; set; }

    [Column]
    public long? DeletedAt { get; set; }
}
