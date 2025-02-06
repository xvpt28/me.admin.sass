using LinqToDB.Mapping;

namespace me.admin.api.Models;

[Table("tblOrder")]
public class Order
{
    [PrimaryKey]
    public required string OrderId { get; set; }

    [Column]
    [NotNull]
    public required string OutletId { get; set; }

    [Column]
    [NotNull]
    public required string OrderStatus { get; set; }

    [Column]
    public float? Discount { get; set; }

    [Column]
    public double? Amount { get; set; }

    [Column]
    public required string Remarks { get; set; }

    [Column]
    public string? PaymentMethod { get; set; }

    [Column]
    [NotNull]
    public required string CreatedBy { get; set; }

    [Column]
    public long? DeletedAt { get; set; }

    [Column]
    [NotNull]
    public required long CreatedAt { get; set; }

    [Column]
    [NotNull]
    public required long UpdatedAt { get; set; }
}
