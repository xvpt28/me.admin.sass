using me.admin.api.Data.Repositories;
using me.admin.api.DTOs;
using me.admin.api.Models;
using Serilog;

namespace me.admin.api.Services;

public class OutletService(OutletRepository outletRepository)
{
	readonly OutletRepository _outletRepository = outletRepository;

	public async Task<BaseResponse<string>> CreateOutlet(CreateOutletRequestDto request)
	{
		try
		{
			var resp = await _outletRepository.GetByOutletName(request.OutletName);
			if (resp != null)
			{
				throw new Exception("OutletName Already Exist");
			}

			var currentTimestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
			var outletId = Ulid.NewUlid().ToString();
			var entity = new Outlet
			{
				OutletId = outletId,
				OutletName = request.OutletName,
				OutletAddress = request.OutletAddress,
				OutletPostcode = request.OutletPostcode,
				OutletPhoneNumber = request.OutletPhoneNumber,
				OutletStatus = request.OutletStatus,
				CreatedAt = currentTimestamp,
				UpdatedAt = currentTimestamp
			};
			await _outletRepository.Insert(entity);
			return new BaseResponse<string>(outletId) { Success = true };
		}
		catch (Exception e)
		{
			Log.Logger.Error(e.Message, "Error creating user");
			return new BaseResponse<string> { Success = false, Message = "Error creating user" };
		}
	}

	public async Task<BaseResponse<Outlet>> GetOutletById(string outletId)
	{
		try
		{
			var outlet = await _outletRepository.GetById(outletId);
			if (outlet == null)
			{
				throw new Exception("Outlet not found");
			}

			return new BaseResponse<Outlet>(outlet) { Success = true };
		}
		catch (Exception e)
		{
			Log.Logger.Error(e.Message, "Error getting outlet by id");
			return new BaseResponse<Outlet> { Success = false, Message = e.Message };
		}
	}

	public async Task<BaseResponse<List<Outlet>>> GetAllOutlets()
	{
		try
		{
			var response = await _outletRepository.GetAll();
			return new BaseResponse<List<Outlet>>(response) { Success = true };
		}
		catch (Exception e)
		{
			Log.Logger.Error(e.Message, "Error getting all outlets");
			return new BaseResponse<List<Outlet>> { Success = false, Message = e.Message };
		}
	}

	public async Task<BaseResponse<bool>> UpdateOutlet(string id, UpdateOutletRequestDto request)
	{
		try
		{
			var response = await _outletRepository.GetById(id);
			if (response == null)
			{
				throw new Exception("Outlet not found");
			}

			var currentTimestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
			var entity = new Outlet
			{
				OutletId = id,
				OutletName = request.OutletName ?? response.OutletName,
				OutletAddress = request.OutletAddress ?? response.OutletName,
				OutletPostcode = request.OutletPostcode ?? response.OutletPostcode,
				OutletPhoneNumber = request.OutletPhoneNumber ?? response.OutletPhoneNumber,
				OutletStatus = request.OutletStatus ?? response.OutletStatus,
				CreatedAt = response.CreatedAt,
				UpdatedAt = currentTimestamp
			};

			await _outletRepository.Update(entity);
			return new BaseResponse<bool>(true) { Success = true };
		}
		catch (Exception e)
		{
			Log.Logger.Error(e.Message, "Error updating outlet");
			return new BaseResponse<bool> { Success = false, Message = e.Message };
		}
	}

	public async Task<BaseResponse<bool>> DeleteOutlet(string id)
	{
		try
		{
			await _outletRepository.Delete(id);
			return new BaseResponse<bool>(true) { Success = true };
		}
		catch (Exception e)
		{
			Log.Logger.Error(e.Message, "Error deleting outlet");
			return new BaseResponse<bool> { Success = false, Message = e.Message };
		}
	}
}