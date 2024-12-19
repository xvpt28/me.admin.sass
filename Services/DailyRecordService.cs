using me.admin.api.Data.Repositories;
using me.admin.api.DTOs;
using me.admin.api.Models;
using Serilog;

namespace me.admin.api.Services;

public class DailyRecordService(DailyRecordRepository dailyRecordRepository, AuthService authService)
{
	readonly AuthService _authService = authService;
	readonly DailyRecordRepository _dailyRecordRepository = dailyRecordRepository;

	public async Task<BaseResponse<string>> CreateDailyRecord(string outletId, CreateDailyRecordRequestDto request)
	{
		try
		{
			var userId = _authService.GetUserId();
			if (userId == null)
			{
				throw new Exception("Invalid user");
			}
			var response = await _dailyRecordRepository.GetByDateAndOutletId(outletId, request.RecordDate);
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
				Revenue = request.Revenue,
				Cash = request.Cash,
				CreatedBy = userId,
				CreatedAt = currentTimestamp,
				UpdatedAt = currentTimestamp
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

	public async Task<BaseResponse<List<GetDailyRecordResponseDto>>> GetAllRecordsByOutlet(string outletId)
	{
		try
		{
			var response = await _dailyRecordRepository.GetAll(outletId);
			return new BaseResponse<List<GetDailyRecordResponseDto>>(response) { Success = true };
		}
		catch (Exception e)
		{
			Log.Logger.Error(e.Message, "Error creating daily record");
			return new BaseResponse<List<GetDailyRecordResponseDto>> { Success = false, Message = e.Message };
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
			return new BaseResponse<List<GetDailyRecordResponseDto>> { Success = false, Message = e.Message };
		}
	}
}