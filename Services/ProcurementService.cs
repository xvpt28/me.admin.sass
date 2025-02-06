using me.admin.api.DTOs;
using me.admin.api.Utils;
using Microsoft.Extensions.Options;
using Serilog;
using Tesseract;

namespace me.admin.api.Services;

public class ProcurementService(AuthService authService, IOptions<FileSetting> fileSetting)
{
	static readonly HashSet<string> AllowedExtensions = new HashSet<string>
	{
		".jpg",
		".jpeg",
		".png",
		".pdf"
	};

	static readonly HashSet<string> AllowedMimeTypes = new HashSet<string>
	{
		"image/jpg",
		"image/jpeg",
		"image/png",
		"application/pdf"
	};

	readonly AuthService _authService = authService;

	readonly FileSetting _fileSetting = fileSetting.Value;

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
			var rootFolder = Path.Combine(_fileSetting.RootFolder, _fileSetting.ProcurementRootFolder);
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
			var dataPath = "/opt/homebrew/share/tessdata/";
			using var engine = new TesseractEngine(dataPath, "eng", EngineMode.Default);
			using var img = Pix.LoadFromFile(filePath);
			using var result = engine.Process(img);
			var text = "识别结果: " + result.GetText();

			return new BaseResponse<string>(text) { Success = true };
		}
		catch (Exception e)
		{
			Log.Logger.Error(e.Message, "Error uploading expense image");
			return new BaseResponse<string> { Success = false, Message = e.Message };
		}
	}
}