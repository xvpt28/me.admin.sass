namespace me.admin.api.DTOs;

public class CreateOutletRequestDto
{
    public required string OutletName { get; set; }
    public required string OutletAddress { get; set; }
    public required string OutletPostcode { get; set; }
    public required string OutletPhoneNumber { get; set; }
    public required string OutletStatus { get; set; }
}

public class UpdateOutletRequestDto
{
    public string? OutletName { get; set; }
    public string? OutletAddress { get; set; }
    public string? OutletPostcode { get; set; }
    public string? OutletPhoneNumber { get; set; }
    public string? OutletStatus { get; set; }
}
