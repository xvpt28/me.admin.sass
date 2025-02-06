using me.admin.api.Data.Repositories;
using me.admin.api.DTOs;
using me.admin.api.Models;
using Serilog;

namespace me.admin.api.Services;

public class ImportanceService(ImportanceRepository importanceRepository, AuthService authService)
{
    readonly AuthService _authService = authService;
    readonly ImportanceRepository _importanceRepository = importanceRepository;

    public async Task<BaseResponse<List<Importance>>> GetAllImportanceByOutlet(string outletId)
    {
        try
        {
            var resp = await _importanceRepository.GetAll(outletId);
            return new BaseResponse<List<Importance>>(resp) { Success = true };
        }
        catch (Exception e)
        {
            Log.Logger.Error(e.Message, "Error getting all importance");
            return new BaseResponse<List<Importance>> { Success = false, Message = e.Message };
        }
    }

    public async Task<BaseResponse<bool>> DeleteImportance(string id)
    {
        try
        {
            await _importanceRepository.Delete(id);
            return new BaseResponse<bool>(true) { Success = true };
        }
        catch (Exception e)
        {
            Log.Logger.Error(e.Message, "Error deleting importance");
            return new BaseResponse<bool> { Success = false, Message = e.Message };
        }
    }

    public async Task<BaseResponse<bool>> UpdateImportanceById(
        string noteId,
        UpdateImportanceDto request
    )
    {
        try
        {
            var importance = await _importanceRepository.GetById(noteId);
            if (importance == null)
                return new BaseResponse<bool> { Success = false, Message = "Importance not found" };
            importance.Title = request.Title ?? importance.Title;
            importance.Content = request.Content ?? importance.Content;
            importance.Type = request.Type ?? importance.Type;
            importance.IsArchived = request.IsArchived ?? importance.IsArchived;
            await _importanceRepository.Update(importance);
            return new BaseResponse<bool>(true) { Success = true };
        }
        catch (Exception e)
        {
            Log.Logger.Error(e.Message, "Error updating importance");
            return new BaseResponse<bool> { Success = false, Message = e.Message };
        }
    }

    public async Task<BaseResponse<bool>> CreateImportanceById(
        string outletId,
        CreateImportanceDto request
    )
    {
        try
        {
            var currentTimestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            var userId = _authService.GetUserId();
            if (userId == null)
                throw new Exception("Invalid user");
            var entity = new Importance
            {
                NoteId = Ulid.NewUlid().ToString(),
                Title = request.Title,
                Content = request.Content,
                Type = request.Type,
                IsArchived = false,
                OutletId = outletId,
                CreatedAt = currentTimestamp,
                UpdatedAt = currentTimestamp,
                CreatedBy = userId,
            };
            await _importanceRepository.Insert(entity);
            return new BaseResponse<bool>(true) { Success = true };
        }
        catch (Exception e)
        {
            Log.Logger.Error(e.Message, "Error updating importance");
            return new BaseResponse<bool> { Success = false, Message = e.Message };
        }
    }
}
