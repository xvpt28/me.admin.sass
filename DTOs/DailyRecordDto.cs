using me.admin.api.Models;

namespace me.admin.api.DTOs;

public class CreateDailyRecordRequestDto
{
	public required long RecordDate { get; set; }
	public required double Revenue { get; set; }
	public required double Cash { get; set; }
}

public class UpdateDailyRecordRequestDto
{
	public long? RecordDate { get; set; }
	public double? Revenue { get; set; }
	public double? Cash { get; set; }
}

public class GetDailyRecordResponseDto : DailyRecord
{
	public required Outlet OutletInfo { get; set; }
	public required GetUserDto UserInfo { get; set; }
}