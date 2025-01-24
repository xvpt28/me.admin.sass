using me.admin.api.DTOs;
using me.admin.api.Services;
using Microsoft.AspNetCore.Mvc;

namespace me.admin.api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class InvoiceController(InvoiceService invoiceService) : ControllerBase
{
	readonly InvoiceService _invoiceService = invoiceService;

	[HttpGet("all/{outletId}")]
	public async Task<IActionResult> GetAllInvoice([FromRoute] string outletId)
	{
		var response = await _invoiceService.GetAllInvoiceRecord(outletId);
		if (response.Success)
			return Ok(response);
		return NotFound(response);
	}

	[HttpGet("download/{invoiceId}")]
	public async Task<IActionResult> GenerateInvoice([FromRoute] int invoiceId)
	{
		var response = await _invoiceService.GetInvoiceById(invoiceId);
		try
		{
			if (response.Success && response.Data != null && response.Data.FilePath != null)
			{
				var rootPath = Directory.GetCurrentDirectory();
				var filePath = Path.Combine(rootPath, response.Data.FilePath);
				if (!System.IO.File.Exists(filePath)) throw new Exception("File not found");
				var downloadName = response.Data.FilePath.Split('/').Last();
				var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
				var contentType = "application/octet-stream"; // 或自定义 MIME 类型
				return File(fileStream, contentType, downloadName);
			}
		}
		catch (Exception ex)
		{
			return NotFound(ex.Message);
		}

		return NotFound(response);
	}

	[HttpPost("generate/{orderId}")]
	public async Task<IActionResult> GenerateInvoice([FromRoute] string orderId, [FromBody] CreateInvoiceDto body)
	{
		var response = await _invoiceService.GenerateInvoice(orderId, body);
		try
		{
			if (response.Success && response.Data != null)
			{
				var rootPath = Directory.GetCurrentDirectory();
				var filePath = Path.Combine(rootPath, response.Data);
				if (!System.IO.File.Exists(filePath)) throw new Exception("File not found");
				var downloadName = response.Data.Split('/').Last();
				var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
				var contentType = "application/octet-stream"; // 或自定义 MIME 类型
				return File(fileStream, contentType, downloadName);
			}
		}
		catch (Exception ex)
		{
			return NotFound(ex.Message);
		}

		return NotFound(response);
	}

	[HttpDelete("{invoiceId}")]
	public async Task<IActionResult> DeleteInvoice([FromRoute] int invoiceId)
	{
		var response = await _invoiceService.DeleteInvoice(invoiceId);
		if (response.Success)
			return Ok(response);
		return NotFound(response);
	}
}