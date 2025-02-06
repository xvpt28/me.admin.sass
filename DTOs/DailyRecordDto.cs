using me.admin.api.Models;

namespace me.admin.api.DTOs;

public class CreateDailyRecordRequestDto
{
    public required long RecordDate { get; set; }
    public required double Revenue { get; set; }
    public required double Cash { get; set; }
}

public class UpdateAdminDailyRecordRequestDto
{
    public double? Revenue { get; set; }
    public double? Cash { get; set; }
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

public class GetDailyRecordFilterDto
{
    public required long StartDate { get; set; }
    public required long EndDate { get; set; }
}

public class GetDailyRecordWithFilterDataDto : DailyRecord
{
    public required GetUserDto UserInfo { get; set; }
}

public class GetDailyRecordWithFilterResponseDto
{
    public required int RevenueId { get; set; }
    public required long RecordDate { get; set; }
    public GetDailyRecordWithFilterDataDto? DailyRecord { get; set; }
    public required double CashIncome { get; set; }
    public required double CashExpense { get; set; }
    public required double CashDiff { get; set; }
}

public class GetMonthlyRecordWithFilterResponseDto
{
    public required int RevenueId { get; set; }
    public required long RecordDate { get; set; }
    public required double Revenue { get; set; }
    public required double WeekdayAvg { get; set; }
    public required double FridayAvg { get; set; }
    public required double SaturdayAvg { get; set; }
    public required double SundayAvg { get; set; }
}
