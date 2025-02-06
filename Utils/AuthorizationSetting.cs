namespace me.admin.api.Utils;

public class AuthorizationSetting
{
    public required string JwtSecret { get; set; }
    public required int JwtExpirationInMinutes { get; set; }
}
