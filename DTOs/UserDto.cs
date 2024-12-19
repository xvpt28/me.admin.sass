namespace me.admin.api.DTOs;

public class CreateUserDto
{
	public required string Email { get; set; }
	public required string Password { get; set; }
	public required string Fullname { get; set; }
	public required string Role { get; set; }
}

public class GetUserDto
{
	public required string UserId { get; set; }
	public required string Email { get; set; }
	public required string Fullname { get; set; }
	public required string Role { get; set; }
}