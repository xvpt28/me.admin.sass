namespace me.admin.api.DTOs;

public class CreateExpenseRecordRequestDto
{
	public required long ClaimDate { get; set; }
	public required double Amount { get; set; }

	public string? Remark { get; set; }

	public required string Type { get; set; }

	public string? FilePaths { get; set; }

	public required bool IsClaimRequired { get; set; }
}

public class UpdateExpenseRecordRequestDto
{
	public required string Type { get; set; }
}

public class DeleteExpenseImage
{
	public required string FilePath { get; set; }
}