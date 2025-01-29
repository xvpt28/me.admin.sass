using me.admin.api.Data.Repositories;
using me.admin.api.DTOs;
using me.admin.api.Models;
using Serilog;

namespace me.admin.api.Services;

public class OrderService(OrderRepository orderRepository, AuthService authService, OrderItemRepository orderItemRepository)
{
	readonly AuthService _authService = authService;
	readonly OrderItemRepository _orderItemRepository = orderItemRepository;
	readonly OrderRepository _orderRepository = orderRepository;

	public async Task<BaseResponse<List<Order>>> GetAllOngoingOrder(string outletId)
	{
		try
		{
			var orders = await _orderRepository.GetAllOngoingOrder(outletId);
			return new BaseResponse<List<Order>>(orders) { Success = true };
		}
		catch (Exception e)
		{
			Log.Logger.Error(e.Message, "Error getting all orders");
			return new BaseResponse<List<Order>> { Success = false, Message = e.Message };
		}
	}

	public async Task<BaseResponse<List<Order>>> GetAllOrdersWithFilters(string outletId, GetOrderFilterDto filters)
	{
		try
		{
			var orders = await _orderRepository.GetAllOrdersWithFilters(outletId, filters);
			return new BaseResponse<List<Order>>(orders) { Success = true };
		}
		catch (Exception e)
		{
			Log.Logger.Error(e.Message, "Error getting all orders");
			return new BaseResponse<List<Order>> { Success = false, Message = e.Message };
		}
	}

	public async Task<BaseResponse<Order>> GetOrderById(string orderId)
	{
		try
		{
			var order = await _orderRepository.GetById(orderId);
			if (order == null)
				return new BaseResponse<Order> { Success = false, Message = "Order not found" };
			return new BaseResponse<Order>(order) { Success = true };
		}
		catch (Exception e)
		{
			Log.Logger.Error(e.Message, "Error getting all orders");
			return new BaseResponse<Order> { Success = false, Message = e.Message };
		}
	}

	public async Task<BaseResponse<string>> CreateOrder(string outletId, CreateOrderDto body)
	{
		try
		{
			var userId = _authService.GetUserId();
			if (userId == null)
				throw new Exception("User not authenticated");
			var orderId = Ulid.NewUlid().ToString();
			var currentTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
			var entity = new Order
			{
				OrderId = orderId,
				OutletId = outletId,
				OrderStatus = body.OrderStatus,
				Remarks = body.Remarks,
				CreatedBy = userId,
				CreatedAt = body.CreatedAt ?? currentTime,
				UpdatedAt = currentTime
			};
			await _orderRepository.Insert(entity);
			return new BaseResponse<string>(orderId) { Success = true };
		}
		catch (Exception e)
		{
			Log.Logger.Error(e.Message, "Error creating order");
			return new BaseResponse<string> { Success = false, Message = e.Message };
		}
	}

	public async Task<BaseResponse<bool>> UpdateOrder(string orderId, UpdateOrderDto body)
	{
		try
		{
			var order = await _orderRepository.GetById(orderId);
			if (order == null)
				throw new Exception("Order not found");
			order.OrderStatus = body.OrderStatus ?? order.OrderStatus;
			order.Discount = body.Discount ?? order.Discount;
			order.Amount = body.Amount ?? order.Amount;
			order.Remarks = body.Remarks ?? order.Remarks;
			order.PaymentMethod = body.PaymentMethod ?? order.PaymentMethod;
			order.UpdatedAt = DateTimeOffset.Now.ToUnixTimeMilliseconds();
			if (order.Amount != null)
			{
				order.Amount = Math.Round(order.Amount.Value, 2);
			}
			await _orderRepository.Update(order);
			return new BaseResponse<bool>(true) { Success = true };
		}
		catch (Exception e)
		{
			Log.Logger.Error(e.Message, "Error updating order");
			return new BaseResponse<bool> { Success = false, Message = e.Message };
		}
	}

	public async Task<BaseResponse<bool>> DeleteOrder(string orderId)
	{
		try
		{
			var resp = await _orderItemRepository.GetAll(orderId);
			await _orderRepository.Delete(orderId);
			foreach (var item in resp)
			{
				await _orderItemRepository.Delete(item.OrderItemId);
			}

			return new BaseResponse<bool>(true) { Success = true };
		}
		catch (Exception e)
		{
			Log.Logger.Error(e.Message, "Error deleting order");
			return new BaseResponse<bool> { Success = false, Message = e.Message };
		}
	}
}