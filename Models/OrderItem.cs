using LinqToDB.Mapping;

namespace me.admin.api.Models;

[Table("tblOrderItem")]
public class OrderItem
{
    [PrimaryKey]
    public required string OrderItemId { get; set; }

    [Column]
    [NotNull]
    public required string OrderId { get; set; }

    [Column]
    [NotNull]
    public required string MenuId { get; set; }

    [Column]
    [NotNull]
    public required int Quantity { get; set; }

    [Column]
    public required double UnitPrice { get; set; }

    [Column]
    public long? DeletedAt { get; set; }

    [Column]
    [NotNull]
    public long? CreatedAt { get; set; }

    [Column]
    [NotNull]
    public long? UpdatedAt { get; set; }
}
