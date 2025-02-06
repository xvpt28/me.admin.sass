using me.admin.api.Data.Repositories;
using me.admin.api.DTOs;
using me.admin.api.Models;
using Serilog;

namespace me.admin.api.Services;

public class DailyRecordService(
    DailyRecordRepository dailyRecordRepository,
    AuthService authService,
    OrderRepository orderRepository,
    ExpenseRepository expenseRepository
)
{
    readonly AuthService _authService = authService;
    readonly DailyRecordRepository _dailyRecordRepository = dailyRecordRepository;
    readonly OrderRepository _orderRepository = orderRepository;
    readonly ExpenseRepository _expenseRepository = expenseRepository;

    public async Task<BaseResponse<string>> CreateDailyRecord(
        string outletId,
        CreateDailyRecordRequestDto request
    )
    {
        try
        {
            var userId = _authService.GetUserId();
            if (userId == null)
            {
                throw new Exception("Invalid user");
            }
            var response = await _dailyRecordRepository.GetByDateAndOutletId(
                outletId,
                request.RecordDate
            );
            if (response != null)
            {
                throw new Exception("Record already exist");
            }
            var id = Ulid.NewUlid().ToString();
            var currentTimestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            var entity = new DailyRecord
            {
                RecordId = id,
                OutletId = outletId,
                RecordDate = request.RecordDate,
                Revenue = Math.Round(request.Revenue, 2),
                Cash = Math.Round(request.Cash, 2),
                CreatedBy = userId,
                CreatedAt = currentTimestamp,
                UpdatedAt = currentTimestamp,
            };
            await _dailyRecordRepository.Insert(entity);
            return new BaseResponse<string>(id) { Success = true };
        }
        catch (Exception e)
        {
            Log.Logger.Error(e.Message, "Error creating daily record");
            return new BaseResponse<string> { Success = false, Message = e.Message };
        }
    }

    public async Task<BaseResponse<bool>> UpdateAdminDailyRecord(
        string recordId,
        UpdateAdminDailyRecordRequestDto request
    )
    {
        try
        {
            var userId = _authService.GetUserId();
            if (userId == null)
                throw new Exception("Invalid user");
            var role = await _authService.GetRole();
            if (role == null)
                throw new Exception("Invalid role");
            var response = await _dailyRecordRepository.GetById(recordId);
            if (response == null)
                throw new Exception("Record not exist");
            if (role == "Manager" && response.CreatedBy != userId)
                throw new Exception("Invalid user");
            var currentTimestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            response.Revenue = Math.Round(request.Revenue ?? response.Revenue, 2);
            response.Cash = Math.Round(request.Cash ?? response.Cash, 2);
            response.UpdatedAt = currentTimestamp;
            await _dailyRecordRepository.Update(response);
            return new BaseResponse<bool>(true) { Success = true };
        }
        catch (Exception e)
        {
            Log.Logger.Error(e.Message, "Error updating daily record");
            return new BaseResponse<bool> { Success = false, Message = e.Message };
        }
    }

    public async Task<
        BaseResponse<List<GetDailyRecordWithFilterResponseDto>>
    > GetAllDailyRecordsByOutletWithFilter(string outletId, GetDailyRecordFilterDto filter)
    {
        try
        {
            var role = await _authService.GetRole();
            if (role == null)
                throw new Exception("Invalid role");
            DateTime startDate;
            DateTime endDate;
            if (role == "Manager")
            {
                startDate = DateTime
                    .Today.AddDays(-5)
                    .AddHours(0)
                    .AddMinutes(0)
                    .AddSeconds(0)
                    .ToLocalTime();
                endDate = DateTime
                    .Today.AddHours(23)
                    .AddMinutes(59)
                    .AddSeconds(59)
                    .AddMilliseconds(999)
                    .ToLocalTime();
                filter.StartDate = new DateTimeOffset(startDate).ToUnixTimeMilliseconds();
                filter.EndDate = new DateTimeOffset(endDate).ToUnixTimeMilliseconds();
            }
            else
            {
                startDate = DateTimeOffset.FromUnixTimeMilliseconds(filter.StartDate).LocalDateTime;
                endDate = DateTimeOffset.FromUnixTimeMilliseconds(filter.EndDate).LocalDateTime;
            }

            var response = await _dailyRecordRepository.GetAllDailyWithFilter(outletId, filter);
            // 转换到需要的格式
            var result = new List<GetDailyRecordWithFilterResponseDto>();

            // 将 respData 转换为字典，以日期为键
            var recordMap = response
                .GroupBy(x =>
                    DateTimeOffset.FromUnixTimeMilliseconds(x.RecordDate).LocalDateTime.Date
                ) // 按日期分组（忽略时间部分）
                .ToDictionary(g => g.Key, g => g.FirstOrDefault());

            // 遍历日期范围并填充数据
            var previousData = await _dailyRecordRepository.GetPreviousDailyRecord(
                outletId,
                filter.StartDate
            ); // 获取上一天的数据()

            var lastData = previousData == null ? null : new { previousData.Cash };
            var id = 0;
            for (
                var currentDate = startDate;
                currentDate <= endDate && currentDate <= DateTime.Now;
                currentDate = currentDate.AddDays(1)
            )
            {
                var startOfDay = currentDate;
                var endOfDay = currentDate.AddDays(1).AddMilliseconds(-1);
                var startTimestamp = new DateTimeOffset(startOfDay).ToUnixTimeMilliseconds();
                var endTimestamp = new DateTimeOffset(endOfDay).ToUnixTimeMilliseconds();
                var orders = await _orderRepository.GetAllOrderPayByCashByDate(
                    outletId,
                    startTimestamp,
                    endTimestamp
                );
                var cashIncome = orders.Sum(x => x.Amount) ?? 0;
                var dateTimeOffset = new DateTimeOffset(currentDate, DateTimeOffset.Now.Offset);
                var dailyRecord = recordMap.TryGetValue(currentDate, out var value) ? value : null;
                var expenses = await _expenseRepository.GetAllByOutletAndDate(
                    outletId,
                    startTimestamp,
                    endTimestamp,
                    false,
                    "cash"
                );
                var cashExpense = expenses.Sum(x => x.Amount);
                double cashDiff = 0;
                if (dailyRecord != null)
                {
                    cashDiff = dailyRecord.Cash - (lastData?.Cash ?? 0);
                    lastData = new { dailyRecord.Cash };
                }
                result.Add(
                    new GetDailyRecordWithFilterResponseDto
                    {
                        RevenueId = id,
                        RecordDate = dateTimeOffset.ToUnixTimeMilliseconds(),
                        DailyRecord = dailyRecord,
                        CashDiff = cashDiff - cashIncome + cashExpense,
                        CashIncome = cashIncome,
                        CashExpense = cashExpense,
                    }
                );
                id++;
            }

            return new BaseResponse<List<GetDailyRecordWithFilterResponseDto>>(result)
            {
                Success = true,
            };
        }
        catch (Exception e)
        {
            Log.Logger.Error(e.Message, "Error creating daily record");
            return new BaseResponse<List<GetDailyRecordWithFilterResponseDto>>
            {
                Success = false,
                Message = e.Message,
            };
        }
    }

    public async Task<
        BaseResponse<List<GetMonthlyRecordWithFilterResponseDto>>
    > GetAllMonthlyRecordsByOutletWithFilter(string outletId, GetDailyRecordFilterDto filter)
    {
        try
        {
            var role = await _authService.GetRole();
            if (role == null || role == "Manager")
                throw new Exception("Invalid role");

            var startDate = DateTimeOffset.FromUnixTimeMilliseconds(filter.StartDate).LocalDateTime;
            var endDate = DateTimeOffset.FromUnixTimeMilliseconds(filter.EndDate).LocalDateTime;
            var response = await _dailyRecordRepository.GetAllDailyWithFilter(outletId, filter);

            // 转换到需要的格式
            var result = new List<GetMonthlyRecordWithFilterResponseDto>();
            var id = 0;
            for (
                var currentDate = startDate;
                currentDate <= endDate && currentDate <= DateTime.Now.ToLocalTime();
                currentDate = currentDate.AddMonths(1)
            )
            {
                var startOfDay = currentDate;
                var endOfDay = currentDate.AddMonths(1).AddMilliseconds(-1);
                var dateTimeOffset = new DateTimeOffset(currentDate, DateTimeOffset.Now.Offset);
                var monthlyRecord = response
                    .Where(x =>
                        DateTimeOffset.FromUnixTimeMilliseconds(x.RecordDate).LocalDateTime
                            >= startOfDay
                        && DateTimeOffset.FromUnixTimeMilliseconds(x.RecordDate).LocalDateTime
                            <= endOfDay
                    )
                    .ToList();
                var weekdayRecord = monthlyRecord
                    .Where(x =>
                        DateTimeOffset
                            .FromUnixTimeMilliseconds(x.RecordDate)
                            .LocalDateTime.DayOfWeek == DayOfWeek.Monday
                        || DateTimeOffset
                            .FromUnixTimeMilliseconds(x.RecordDate)
                            .LocalDateTime.DayOfWeek == DayOfWeek.Tuesday
                        || DateTimeOffset
                            .FromUnixTimeMilliseconds(x.RecordDate)
                            .LocalDateTime.DayOfWeek == DayOfWeek.Wednesday
                        || DateTimeOffset
                            .FromUnixTimeMilliseconds(x.RecordDate)
                            .LocalDateTime.DayOfWeek == DayOfWeek.Thursday
                    )
                    .ToList();
                var fridayRecord = monthlyRecord
                    .Where(x =>
                        DateTimeOffset
                            .FromUnixTimeMilliseconds(x.RecordDate)
                            .LocalDateTime.DayOfWeek == DayOfWeek.Friday
                    )
                    .ToList();
                var saturdayRecord = monthlyRecord
                    .Where(x =>
                        DateTimeOffset
                            .FromUnixTimeMilliseconds(x.RecordDate)
                            .LocalDateTime.DayOfWeek == DayOfWeek.Saturday
                    )
                    .ToList();
                var sundayRecord = monthlyRecord
                    .Where(x =>
                        DateTimeOffset
                            .FromUnixTimeMilliseconds(x.RecordDate)
                            .LocalDateTime.DayOfWeek == DayOfWeek.Sunday
                    )
                    .ToList();
                result.Add(
                    new GetMonthlyRecordWithFilterResponseDto
                    {
                        RevenueId = id,
                        RecordDate = dateTimeOffset.ToUnixTimeMilliseconds(),
                        Revenue = monthlyRecord.Sum(x => x.Revenue),
                        WeekdayAvg =
                            weekdayRecord.Count == 0
                                ? 0
                                : weekdayRecord.Sum(x => x.Revenue) / weekdayRecord.Count,
                        FridayAvg =
                            fridayRecord.Count == 0
                                ? 0
                                : fridayRecord.Sum(x => x.Revenue) / fridayRecord.Count,
                        SaturdayAvg =
                            saturdayRecord.Count == 0
                                ? 0
                                : saturdayRecord.Sum(x => x.Revenue) / saturdayRecord.Count,
                        SundayAvg =
                            sundayRecord.Count == 0
                                ? 0
                                : sundayRecord.Sum(x => x.Revenue) / sundayRecord.Count,
                    }
                );
                id++;
            }

            return new BaseResponse<List<GetMonthlyRecordWithFilterResponseDto>>(result)
            {
                Success = true,
            };
        }
        catch (Exception e)
        {
            Log.Logger.Error(e.Message, "Error creating monthly record");
            return new BaseResponse<List<GetMonthlyRecordWithFilterResponseDto>>
            {
                Success = false,
                Message = e.Message,
            };
        }
    }

    public async Task<BaseResponse<List<GetDailyRecordResponseDto>>> GetAllRecordsByDate(long date)
    {
        try
        {
            var response = await _dailyRecordRepository.GetAllByDate(date);
            return new BaseResponse<List<GetDailyRecordResponseDto>>(response) { Success = true };
        }
        catch (Exception e)
        {
            Log.Logger.Error(e.Message, "Error creating daily record");
            return new BaseResponse<List<GetDailyRecordResponseDto>>
            {
                Success = false,
                Message = e.Message,
            };
        }
    }

    public async Task<BaseResponse<DailyRecord>> GetRecordByOutletAndDate(
        string outletId,
        long date
    )
    {
        try
        {
            var response = await _dailyRecordRepository.GetByDateAndOutletId(outletId, date);
            if (response == null)
                throw new Exception("Record not exist");
            return new BaseResponse<DailyRecord>(response) { Success = true };
        }
        catch (Exception e)
        {
            Log.Logger.Error(e.Message, "Error creating daily record");
            return new BaseResponse<DailyRecord> { Success = false, Message = e.Message };
        }
    }

    public async Task<BaseResponse<bool>> UpdateDailyRecord(
        string dailyRecordId,
        UpdateDailyRecordRequestDto request
    )
    {
        try
        {
            var response = await _dailyRecordRepository.GetById(dailyRecordId);
            if (response == null)
            {
                throw new Exception("Record is not found");
            }
            var currentTimestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            var entity = new DailyRecord
            {
                RecordId = dailyRecordId,
                OutletId = response.OutletId,
                RecordDate = request.RecordDate ?? response.RecordDate,
                Revenue = request.Revenue ?? response.Revenue,
                Cash = request.Cash ?? response.Cash,
                CreatedBy = response.CreatedBy,
                CreatedAt = response.CreatedAt,
                UpdatedAt = currentTimestamp,
            };
            entity.Revenue = Math.Round(entity.Revenue, 2);
            entity.Cash = Math.Round(entity.Cash, 2);
            await _dailyRecordRepository.Update(entity);
            return new BaseResponse<bool>(true) { Success = true };
        }
        catch (Exception e)
        {
            Log.Logger.Error(e.Message, "Error updating daily record");
            return new BaseResponse<bool> { Success = false, Message = e.Message };
        }
    }

    public async Task<BaseResponse<bool>> DeleteDailyRecord(string id)
    {
        try
        {
            await _dailyRecordRepository.Delete(id);
            return new BaseResponse<bool>(true) { Success = true };
        }
        catch (Exception e)
        {
            Log.Logger.Error(e.Message, "Error deleting daily record");
            return new BaseResponse<bool> { Success = false, Message = e.Message };
        }
    }
}
