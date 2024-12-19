namespace me.admin.api.DTOs;

public class CreateOutletRequestDto
{
	public required string OutletName { get; set; }
	public required string OutletAddress { get; set; }
	public required string OutletPostcode { get; set; }
	public required string OutletPhoneNumber { get; set; }
	public required string OutletStatus { get; set; }
}