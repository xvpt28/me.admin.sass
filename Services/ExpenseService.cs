using me.admin.api.Data.Repositories;
using me.admin.api.DTOs;
using me.admin.api.Models;
using me.admin.api.Utils;
using Microsoft.Extensions.Options;
using Serilog;

namespace me.admin.api.Services;

public class ExpenseService(
    ExpenseRepository expenseRepository,
    AuthService authService,
    IOptions<FileSetting> fileSetting
)
{
    static readonly HashSet<string> AllowedExtensions = new HashSet<string>
    {
        ".jpg",
        ".jpeg",
        ".png",
        ".pdf",
    };

    static readonly HashSet<string> AllowedMimeTypes = new HashSet<string>
    {
        "image/jpg",
        "image/jpeg",
        "image/png",
        "application/pdf",
    };

    readonly AuthService _authService = authService;
    readonly ExpenseRepository _expenseRepository = expenseRepository;
    readonly FileSetting _fileSetting = fileSetting.Value;

    public async Task<BaseResponse<List<Expense>>> GetAllExpenseRecordsByOutletAndDate(
        string outletId,
        long startDate,
        long endDate,
        bool isClaimRequired,
        string method
    )
    {
        try
        {
            var response = await _expenseRepository.GetAllByOutletAndDate(
                outletId,
                startDate,
                endDate,
                isClaimRequired,
                method
            );
            return new BaseResponse<List<Expense>>(response) { Success = true };
        }
        catch (Exception e)
        {
            Log.Logger.Error(e.Message, "Error getting expense records");
            return new BaseResponse<List<Expense>> { Success = false, Message = e.Message };
        }
    }

    public async Task<BaseResponse<string>> CreateExpenseRecord(
        string outletId,
        CreateExpenseRecordRequestDto request
    )
    {
        try
        {
            var userId = _authService.GetUserId();
            if (userId == null)
                throw new Exception("Invalid user");
            var id = Ulid.NewUlid().ToString();
            var currentTimestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            var entity = new Expense
            {
                ExpenseId = id,
                OutletId = outletId,
                ClaimDate = request.ClaimDate,
                Amount = request.Amount,
                Remark = request.Remark,
                Status = "submitted",
                Type = request.Type,
                Method = request.Method,
                FilePaths = request.FilePaths,
                IsClaimRequired = request.IsClaimRequired,
                ClaimBy = userId,
                CreatedAt = currentTimestamp,
                UpdatedAt = currentTimestamp,
            };
            await _expenseRepository.Insert(entity);
            return new BaseResponse<string>(id) { Success = true };
        }
        catch (Exception e)
        {
            Log.Logger.Error(e.Message, "Error creating expense record");
            return new BaseResponse<string> { Success = false, Message = e.Message };
        }
    }

    public async Task<BaseResponse<bool>> UpdateExpenseRecord(
        string expenseId,
        UpdateExpenseRecordRequestDto request
    )
    {
        try
        {
            var userId = _authService.GetUserId();
            if (userId == null)
                throw new Exception("Invalid user");
            var response = await _expenseRepository.GetById(expenseId);
            if (response == null)
                throw new Exception("Record is not found");
            var currentTimestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            response.UpdatedBy = userId;
            response.UpdatedAt = currentTimestamp;
            response.Type = request.Type;
            await _expenseRepository.Update(response);
            return new BaseResponse<bool>(true) { Success = true };
        }
        catch (Exception e)
        {
            Log.Logger.Error(e.Message, "Error updating expense record");
            return new BaseResponse<bool> { Success = false, Message = e.Message };
        }
    }

    public async Task<BaseResponse<bool>> DeleteExpenseRecord(string expenseId)
    {
        try
        {
            var userId = _authService.GetUserId();
            var role = await _authService.GetRole();

            if (userId == null)
                throw new Exception("Invalid user");

            var response = await _expenseRepository.GetById(expenseId);

            if (response == null)
                throw new Exception("Expense record is not found");

            if (response.ClaimBy != userId || role != "SuperAdmin")
                throw new Exception("You are not allowed to delete this expense record");

            await _expenseRepository.Delete(expenseId);

            if (!string.IsNullOrEmpty(response.FilePaths))
            {
                var filePaths = response.FilePaths?.Split(',');
                if (filePaths != null)
                {
                    foreach (var filePath in filePaths)
                    {
                        DeleteImage(filePath);
                    }
                }
            }

            return new BaseResponse<bool>(true) { Success = true };
        }
        catch (Exception e)
        {
            Log.Logger.Error(e.Message, "Error delete expense record");
            return new BaseResponse<bool> { Success = false, Message = e.Message };
        }
    }

    public async Task<BaseResponse<string>> UploadImage(IFormFile file)
    {
        try
        {
            if (file.Length == 0)
                throw new Exception("Invalid file");

            var extension = Path.GetExtension(file.FileName).ToLower();
            var mimeType = file.ContentType.ToLower();

            // 校验文件类型
            if (!AllowedExtensions.Contains(extension) || !AllowedMimeTypes.Contains(mimeType))
            {
                throw new Exception("Invalid Image format");
            }

            // 生成唯一文件名
            var rootFolder = Path.Combine(_fileSetting.RootFolder, _fileSetting.ExpenseRootFolder);
            if (!Directory.Exists(rootFolder))
                Directory.CreateDirectory(rootFolder);
            var fileName = $"{Guid.NewGuid()}_{file.FileName}";
            var relativeUploadFolder = DateTime.UtcNow.ToLocalTime().ToString("yyyyMMdd");
            var uploadFolder = Path.Combine(rootFolder, relativeUploadFolder);
            if (!Directory.Exists(uploadFolder))
            {
                Directory.CreateDirectory(uploadFolder);
            }

            var filePath = Path.Combine(uploadFolder, fileName);

            await using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var relativeFilePath = Path.Combine(relativeUploadFolder, fileName);

            return new BaseResponse<string>(relativeFilePath) { Success = true };
        }
        catch (Exception e)
        {
            Log.Logger.Error(e.Message, "Error uploading expense image");
            return new BaseResponse<string> { Success = false, Message = e.Message };
        }
    }

    public BaseResponse<bool> DeleteImage(string filePath)
    {
        try
        {
            var absPath = Path.Combine(
                _fileSetting.RootFolder,
                _fileSetting.ExpenseRootFolder,
                filePath
            );

            if (string.IsNullOrEmpty(absPath) || !File.Exists(absPath))
            {
                throw new Exception("File not found");
            }
            var directoryPath = Path.GetDirectoryName(absPath);

            File.Delete(absPath);

            // 检查文件夹是否为空
            if (
                Directory.Exists(directoryPath)
                && !Directory.EnumerateFileSystemEntries(directoryPath).Any()
            )
            {
                Directory.Delete(directoryPath);
            }

            return new BaseResponse<bool>(true) { Success = true };
        }
        catch (Exception e)
        {
            Log.Logger.Error(e.Message, "Error deleting expense image");
            return new BaseResponse<bool> { Success = false, Message = e.Message };
        }
    }
}
