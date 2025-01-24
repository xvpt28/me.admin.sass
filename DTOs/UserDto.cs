namespace me.admin.api.DTOs;

public class CreateUserDto
{
	public required string Email { get; set; }
	public required string Password { get; set; }
	public required string FullName { get; set; }
	public required string Role { get; set; }
}

public class GetUserDto
{
	public required string UserId { get; set; }
	public required string Email { get; set; }
	public required string FullName { get; set; }
	public required string Role { get; set; }
}

public class UpdateUserDto
{
	public string? FullName { get; set; }
	public string? Email { get; set; }
	public string? Role { get; set; }
	public bool? IsActive { get; set; }
}

public class UpdateUserPasswordDto
{
	public required string CurrentPassword { get; set; }
	public required string Password { get; set; }
}