namespace me.admin.api.DTOs;

public class LoginRequestDto
{
	public required string Email { get; set; }
	public required string Password { get; set; }
}

public class LoginTokenDto
{
	public required string AccessToken { get; set; }
	public required long Expiry { get; set; }
}

public class UserInfoForTokenDto
{
	public required string UserId { get; set; }
	public required string Email { get; set; }
	public required string FullName { get; set; }
	public required string Role { get; set; }
}