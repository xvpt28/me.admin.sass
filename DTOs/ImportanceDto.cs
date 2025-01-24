namespace me.admin.api.DTOs;

public class CreateImportanceDto
{
	public required string Title { get; set; }
	public required string Content { get; set; }
	public required string Type { get; set; }
}

public class UpdateImportanceDto
{
	public string? Title { get; set; }
	public string? Content { get; set; }
	public string? Type { get; set; }
	public bool? IsArchived { get; set; }
}