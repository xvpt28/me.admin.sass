using System.Globalization;
using System.Text;
using iTextSharp.text;
using iTextSharp.text.pdf;
using me.admin.api.Data.Repositories;
using me.admin.api.DTOs;
using me.admin.api.Models;
using Serilog;

namespace me.admin.api.Services;

public class InvoiceService(
	InvoiceRepository invoiceRepository,
	AuthService authService,
	OrderRepository orderRepository,
	OrderItemRepository orderItemRepository,
	OutletRepository outletRepository)
{
	readonly AuthService _authService = authService;
	readonly InvoiceRepository _invoiceRepository = invoiceRepository;
	readonly OrderItemRepository _orderItemRepository = orderItemRepository;
	readonly OrderRepository _orderRepository = orderRepository;
	readonly OutletRepository _outletRepository = outletRepository;

	public async Task<BaseResponse<List<GetAllInvoiceResponseDto>>> GetAllInvoiceRecord(string outletId, GetInvoiceFilterDto filter)
	{
		try
		{
			var resp = await _invoiceRepository.GetAllWithFilter(outletId, filter);
			return new BaseResponse<List<GetAllInvoiceResponseDto>>(resp) { Success = true };
		}
		catch (Exception e)
		{
			Log.Logger.Error(e.Message, "Error getting all invoice");
			return new BaseResponse<List<GetAllInvoiceResponseDto>> { Success = false, Message = e.Message };
		}
	}

	public async Task<BaseResponse<string>> GenerateInvoice(string orderId, CreateInvoiceDto body)
	{
		try
		{
			var userId = _authService.GetUserId();
			if (userId == null) throw new Exception("User not authenticated");

			var resp = await _invoiceRepository.GetByOrderId(orderId);
			if (resp != null)
			{
				if (resp.FilePath != null && File.Exists(resp.FilePath)) File.Delete(resp.FilePath);
				await _invoiceRepository.Delete(resp.InvoiceId);
			}

			var order = await _orderRepository.GetById(orderId);
			if (order == null) throw new Exception("Order not found");

			var outlet = await _outletRepository.GetById(order.OutletId);
			if (outlet == null) throw new Exception("Outlet not found");

			var orderItem = await _orderItemRepository.GetAllWithMenu(orderId);
			if (orderItem == null) throw new Exception("Order item not found");

			var currentTimestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
			var entity = new Invoice
			{
				OrderId = order.OrderId,
				FilePath = null,
				Type = body.Type,
				BilledTo = body.BilledTo,
				BilledCompanyAddress = body.BilledCompanyAddress,
				BilledCompanyUEN = body.BilledCompanyUEN,
				IssuedDate = currentTimestamp,
				CreatedBy = userId,
				CreatedAt = currentTimestamp,
				UpdatedAt = currentTimestamp,
				DeletedAt = null
			};

			await _invoiceRepository.Insert(entity);

			var invoiceRecord = await _invoiceRepository.GetByOrderId(orderId);
			if (invoiceRecord == null) throw new Exception("Invoice not found");

			var filePath = GenerateInvoiceFile(order, orderItem, outlet, entity, invoiceRecord.InvoiceId, body);

			invoiceRecord.FilePath = filePath;
			await _invoiceRepository.Update(invoiceRecord);

			return new BaseResponse<string>(filePath) { Success = true };
		}
		catch (Exception e)
		{
			Log.Logger.Error(e.Message, "Error getting invoice");
			return new BaseResponse<string> { Success = false, Message = e.Message };
		}
	}

	public async Task<BaseResponse<Invoice>> GetInvoiceById(int invoiceId)
	{
		try
		{
			var resp = await _invoiceRepository.GetById(invoiceId);
			if (resp == null) throw new Exception("Invoice not found");
			return new BaseResponse<Invoice>(resp) { Success = true };
		}
		catch (Exception e)
		{
			Log.Logger.Error(e.Message, "Error getting invoice by id");
			return new BaseResponse<Invoice> { Success = false, Message = e.Message };
		}
	}

	public async Task<BaseResponse<bool>> DeleteInvoice(int invoiceId)
	{
		try
		{
			var resp = await _invoiceRepository.GetById(invoiceId);
			if (resp == null) throw new Exception("Invoice not found");
			if (resp.FilePath != null) File.Delete(resp.FilePath);
			await _invoiceRepository.Delete(invoiceId);
			return new BaseResponse<bool>(true) { Success = true };
		}
		catch (Exception e)
		{
			Log.Logger.Error(e.Message, "Error deleting invoice");
			return new BaseResponse<bool> { Success = false, Message = e.Message };
		}
	}

	//----------------------------------------------------------------------------------------------------------------------
	// @ Private Methods
	//----------------------------------------------------------------------------------------------------------------------
	string GenerateInvoiceFile(
		Order order,
		List<OrderItemWithMenu> orderItems,
		Outlet outlet,
		Invoice data,
		int invoiceId,
		CreateInvoiceDto payload)
	{
		if (!Directory.Exists("Invoice")) Directory.CreateDirectory("Invoice");
		var formattedInvoiceId = invoiceId.ToString("D4");
		var fileName = data.Type == "invoice" ? $"ME-INV-{formattedInvoiceId}.pdf" : $"ME-QUO-{formattedInvoiceId}.pdf";
		var filePath = Path.Combine("Invoice", fileName);
		if (File.Exists(filePath)) File.Delete(filePath);

		Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

		var fontPathBold = "static/font/Montserrat-Bold.ttf"; // 字体文件路径
		var fontPathSemiBold = "static/font/Montserrat-SemiBold.ttf"; // 字体文件路径
		var fontPathRegular = "static/font/Montserrat-Regular.ttf"; // 字体文件路径
		if (!File.Exists(fontPathRegular) || !File.Exists(fontPathBold) || !File.Exists(fontPathSemiBold)) throw new Exception("Font file not found");
		var baseFontNormal = BaseFont.CreateFont(fontPathRegular, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
		var baseFontSemiBold = BaseFont.CreateFont(fontPathSemiBold, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
		var baseFontBold = BaseFont.CreateFont(fontPathBold, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);

		// Create PDF file
		var document = new Document(PageSize.A4, 50, 50, 50, 50);
		var writer = PdfWriter.GetInstance(document, new FileStream(filePath, FileMode.Create));
		document.Open();

		var meFont = new Font(baseFontSemiBold, 12, Font.BOLD, BaseColor.DARK_GRAY);
		var titleFont = new Font(baseFontBold, 22, Font.BOLD, BaseColor.DARK_GRAY);
		var textFont = new Font(baseFontNormal, 10, Font.BOLD, BaseColor.DARK_GRAY);
		var textFontBold = new Font(baseFontBold, 10, Font.BOLD, BaseColor.DARK_GRAY);
		var textFooter = new Font(baseFontNormal, 8, Font.BOLD, BaseColor.DARK_GRAY);
		var textInfo = new CultureInfo("en-US", false).TextInfo;

		var me = new Paragraph("ME CAFE & GAMES LP", meFont);
		document.Add(me);

		var title = new Paragraph(textInfo.ToTitleCase(data.Type), titleFont);
		title.SpacingAfter = 10f;
		document.Add(title);

		// business info
		var issueDate = DateTimeOffset.FromUnixTimeMilliseconds(data.IssuedDate).ToLocalTime().DateTime;
		var issueDateFormatted = issueDate.ToString("dd MMM yyyy", CultureInfo.InvariantCulture);
		var meInfo = new List<string>
		{
			$"{textInfo.ToTitleCase(textInfo.ToTitleCase(data.Type))} ID: {formattedInvoiceId}",
			$"Issue Date: {issueDateFormatted}",
			"UEN: T22LP0078A"
		};
		meInfo.ForEach(x =>
		{
			var text = new Paragraph(x, textFont);
			text.SpacingBefore = 5f;
			document.Add(text);
		});

		var logoPath = "static/images/logo.png";
		if (!File.Exists(logoPath)) throw new Exception("Logo file not found");
		var image = Image.GetInstance(logoPath);
		image.ScaleToFit(100f, 100f);
		image.SetAbsolutePosition(400f, 700f);

		document.Add(image);

		var table = new PdfPTable(4);

		table.SpacingBefore = 30f;
		table.WidthPercentage = 100;

		var tableHeader = new List<string> { "Item Description", "QTY", "Unit Price", "Total" };
		var index = 0;
		foreach (var x in tableHeader)
		{
			var cell = new PdfPCell(new Phrase(x, textFontBold));
			if (index == 0)
			{
				cell.HorizontalAlignment = Element.ALIGN_LEFT;
			}

			else if (index == 3)
			{
				cell.HorizontalAlignment = Element.ALIGN_RIGHT;
			}
			else
			{
				cell.HorizontalAlignment = Element.ALIGN_CENTER;
			}
			cell.Padding = 10f;
			cell.BorderWidth = 0;
			cell.BorderWidthTop = 1f;
			cell.BorderWidthBottom = 1f;
			table.AddCell(cell);
			index++;
		}

		orderItems.ForEach(item =>
		{
			var total = item.Quantity * item.UnitPrice;
			var unitPrice = item.UnitPrice;
			if (payload.HideDiscount)
			{
				var discount = payload.Discount ?? order.Discount ?? 0;
				total *= (100 - discount) / 100;
				unitPrice *= (100 - discount) / 100;
			}
			var totalFormatted = total.ToString("N2", CultureInfo.InvariantCulture);
			var tableContent = new List<string>
			{
				textInfo.ToTitleCase(item.MenuName),
				item.Quantity.ToString(),
				"$" + unitPrice.ToString("N2", CultureInfo.InvariantCulture),
				"$" + totalFormatted
			};

			var idx = 0;
			foreach (var x in tableContent)
			{
				var cell = new PdfPCell(new Phrase(x, textFont));

				if (idx == 0)
				{
					cell.HorizontalAlignment = Element.ALIGN_LEFT;
				}
				else if (idx == 3)
				{
					cell.HorizontalAlignment = Element.ALIGN_RIGHT;
				}
				else
				{
					cell.HorizontalAlignment = Element.ALIGN_CENTER;
				}
				cell.Padding = 10f;
				cell.BorderWidth = 0;
				cell.BorderWidthBottom = 0.5f;
				table.AddCell(cell);
				idx++;
			}
		});

		if (!payload.HideDiscount)
		{
			double subTotal;
			if (payload.Amount != null && payload.Discount != null)
			{
				subTotal = (payload.Amount ?? 0) * ((100 + (payload.Discount ?? 0)) / 100);
			}
			else
			{
				subTotal = (order.Amount ?? 0) * ((100 + (order.Discount ?? 0)) / 100);
			}

			var tableSubTotal = new List<string> { "", "", "Sub Total", "$" + subTotal.ToString("N2", CultureInfo.InvariantCulture) };
			index = 0;
			foreach (var x in tableSubTotal)
			{
				var cell = new PdfPCell(new Phrase(x, textFont));
				if (index == 3 || index == 2)
				{
					cell.HorizontalAlignment = Element.ALIGN_RIGHT;
				}
				else
				{
					cell.HorizontalAlignment = Element.ALIGN_CENTER;
				}

				cell.Padding = 8f;
				cell.BorderWidth = 0;
				cell.BorderWidthTop = 1f;
				table.AddCell(cell);
				index++;
			}
		}

		if ((payload.Discount is > 0 || order.Discount is > 0) && !payload.HideDiscount)
		{
			double discount;
			if (payload.Discount != null && payload.Amount != null)
			{
				discount = (payload.Amount ?? 0) * ((payload.Discount ?? 0) / 100);
			}
			else
			{
				discount = (order.Amount ?? 0) * ((order.Discount ?? 0) / 100);
			}

			var tableDiscount = new List<string> { "", "", "Discount", "$" + discount.ToString("N2", CultureInfo.InvariantCulture) };
			index = 0;
			foreach (var x in tableDiscount)
			{
				var cell = new PdfPCell(new Phrase(x, textFont));

				if (index == 3 || index == 2)
				{
					cell.HorizontalAlignment = Element.ALIGN_RIGHT;
				}
				else
				{
					cell.HorizontalAlignment = Element.ALIGN_CENTER;
				}

				cell.Padding = 8f;
				cell.BorderWidth = 0;
				table.AddCell(cell);
				index++;
			}
		}

		var grandTotal = payload.Amount ?? order.Amount ?? 0;
		var tableGrandTotal = new List<string> { "", "", "Grand Total", "$" + grandTotal.ToString("N2", CultureInfo.InvariantCulture) };
		index = 0;
		foreach (var x in tableGrandTotal)
		{
			var cell = new PdfPCell(new Phrase(x, textFontBold));

			if (index == 3 || index == 2)
			{
				cell.HorizontalAlignment = Element.ALIGN_RIGHT;
			}
			else
			{
				cell.HorizontalAlignment = Element.ALIGN_CENTER;
			}

			cell.Padding = 8f;
			cell.BorderWidth = 0;
			cell.BorderWidthBottom = 1f;
			table.AddCell(cell);
			index++;
		}

		document.Add(table);

		var infoTable = new PdfPTable(2);
		infoTable.SpacingBefore = 20f;
		infoTable.WidthPercentage = 100;
		infoTable.SetWidths(new[] { 50f, 50f });

		// 第一列内容

		var leftCell = new PdfPCell();
		if (data.Type != "quotation")
		{
			var paymentText = new Paragraph("Payment Information", textFontBold);
			leftCell.AddElement(paymentText);
			var paymentInfo = new List<string>
			{
				"ME Cafe Games LP",
				"United Overseas Bank",
				"Acct. No. 4223198788"
			};

			if (order.UpdatedAt > 0)
			{
				var paymentDate = DateTimeOffset.FromUnixTimeMilliseconds(order.UpdatedAt).ToLocalTime().DateTime;
				var paymentDateFormatted = paymentDate.ToString("dd MMM yyyy", CultureInfo.InvariantCulture);
				paymentInfo.Add("Date: " + paymentDateFormatted);
			}

			foreach (var info in paymentInfo)
			{
				var text = new Paragraph(info, textFont);
				text.SpacingBefore = 5f;
				leftCell.AddElement(text);
			}
		}
		leftCell.HorizontalAlignment = Element.ALIGN_LEFT; // 左对齐
		leftCell.Padding = 8f;
		leftCell.BorderWidth = 0;
		infoTable.AddCell(leftCell);

		// 第二列内容
		var rightCell = new PdfPCell();
		if (!string.IsNullOrEmpty(data.BilledTo) || !string.IsNullOrEmpty(data.BilledCompanyAddress) || !string.IsNullOrEmpty(data.BilledCompanyUEN))
		{
			var billedText = new Paragraph("Billed to", textFontBold);
			rightCell.AddElement(billedText);
		}

		var billedInfo = new List<string>();
		if (data.BilledTo != null)
		{
			billedInfo.Add(textInfo.ToTitleCase(data.BilledTo));
		}

		if (data.BilledCompanyAddress != null)
		{
			billedInfo.Add(textInfo.ToTitleCase(data.BilledCompanyAddress));
		}

		if (data.BilledCompanyUEN != null)
		{
			billedInfo.Add(data.BilledCompanyUEN.ToUpper());
		}

		foreach (var info in billedInfo)
		{
			var text = new Paragraph(info, textFont);
			text.SpacingBefore = 5f;
			rightCell.AddElement(text);
		}
		rightCell.HorizontalAlignment = Element.ALIGN_LEFT; // 左对齐
		rightCell.Padding = 8f;
		rightCell.BorderWidth = 0;
		infoTable.AddCell(rightCell);
		document.Add(infoTable);

		var canvas = writer.DirectContent;
		canvas.BeginText();
		canvas.SetFontAndSize(baseFontBold, 10);
		canvas.SetTextMatrix(365, 210); // X=36, Y=36 即底部边缘的 36 点处
		canvas.ShowText("Signature & Company stamp".ToUpper());
		canvas.EndText();

		canvas.SetLineWidth(1f);
		float x1 = 350; // 起点 X 坐标
		float y1 = 120; // 起点 Y 坐标
		float x2 = 560; // 终点 X 坐标
		float y2 = 120; // 终点 Y 坐标
		canvas.MoveTo(x1, y1); // 移动到起点
		canvas.LineTo(x2, y2); // 画到终点
		canvas.Stroke(); // 结束并绘制线条

		var stampPath = "static/images/stamp.png";
		if (!File.Exists(stampPath)) throw new Exception("Stamp file not found");
		var imageStamp = Image.GetInstance(stampPath);
		imageStamp.ScaleToFit(80f, 80f);
		imageStamp.SetAbsolutePosition(415f, 125f);
		document.Add(imageStamp);

		writer.PageEvent = new Footer($"{outlet.OutletAddress}, {outlet.OutletPostcode}", outlet.OutletPhoneNumber, textFooter);

		document.Close();
		return filePath;
	}
}

public class Footer(string location, string contact, Font font) : PdfPageEventHelper
{
	readonly string _contact = contact;
	readonly string _location = location;
	readonly Font _textFont = font;

	public override void OnEndPage(PdfWriter writer, Document document)
	{
		var info = new List<string>
		{
			_location,
			_contact,
			"https://www.mecafegames.com"
		};
		var footerTable = new PdfPTable(1);
		footerTable.TotalWidth = document.PageSize.Width - document.LeftMargin - document.RightMargin;
		footerTable.HorizontalAlignment = Element.ALIGN_CENTER;
		footerTable.DefaultCell.Border = Rectangle.NO_BORDER;
		var cell = new PdfPCell();
		cell.Border = Rectangle.NO_BORDER;
		foreach (var i in info)
		{
			var text = new Paragraph(i, _textFont);
			text.SpacingBefore = 1f;
			text.Alignment = Element.ALIGN_CENTER;
			cell.AddElement(text);
		}

		footerTable.AddCell(cell);
		footerTable.WriteSelectedRows(0, -1, document.LeftMargin, 80, writer.DirectContent);
	}
}