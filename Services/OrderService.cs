using me.admin.api.Data.Repositories;
using me.admin.api.DTOs;
using me.admin.api.Models;
using Serilog;

namespace me.admin.api.Services;

public class OrderService(OrderRepository orderRepository)
{
    readonly OrderRepository _orderRepository = orderRepository;

    public async Task<BaseResponse<List<Order>>> GetAllOrder(string outletId)
    {
        try
        {
            var orders = await _orderRepository.GetAll(outletId);
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
}
