using LinqToDB.Mapping;

namespace me.admin.api.Models;

[Table("tblUser")]
public class User
{
    [PrimaryKey]
    public required string UserId { get; set; }

    [PrimaryKey]
    [NotNull]
    public required string Email { get; set; }

    [Column]
    [NotNull]
    public required string PasswordHash { get; set; }

    [Column]
    [NotNull]
    public required string FullName { get; set; }

    [Column]
    public string? Phone { get; set; }

    [Column]
    [NotNull]
    public long? CreatedAt { get; set; }

    [Column]
    [NotNull]
    public long? UpdatedAt { get; set; }

    [Column]
    public long? LastLogin { get; set; }

    [Column]
    public long? DeletedAt { get; set; }

    [Column]
    [NotNull]
    public required string Role { get; set; }

    [Column]
    [NotNull]
    public bool? IsActive { get; set; } = false;

    [Column]
    public required string ActivateToken { get; set; }

    [Column]
    public required long ActivateTokenGeneratedAt { get; set; }

    [Column]
    public required long ActivateTokenExpiresAt { get; set; }
}
