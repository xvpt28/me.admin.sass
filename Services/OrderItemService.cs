using me.admin.api.Data.Repositories;
using me.admin.api.DTOs;
using me.admin.api.Models;
using Serilog;

namespace me.admin.api.Services;

public class OrderItemService(OrderItemRepository orderItemRepository)
{
    readonly OrderItemRepository _orderItemRepository = orderItemRepository;

    public async Task<BaseResponse<List<OrderItem>>> GetAllOrderItems(string orderId)
    {
        try
        {
            var orderItems = await _orderItemRepository.GetAll(orderId);
            return new BaseResponse<List<OrderItem>>(orderItems) { Success = true };
        }
        catch (Exception e)
        {
            Log.Logger.Error(e.Message, "Error getting order items");
            return new BaseResponse<List<OrderItem>> { Success = false, Message = e.Message };
        }
    }

    public async Task<BaseResponse<bool>> CreateOrderItem(string orderId, CreateOrderItemDto body)
    {
        try
        {
            var currentTimestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            var entity = new OrderItem
            {
                OrderItemId = Ulid.NewUlid().ToString(),
                OrderId = orderId,
                MenuId = body.MenuId,
                UnitPrice = Math.Round(body.UnitPrice, 2),
                Quantity = 1,
                CreatedAt = currentTimestamp,
                UpdatedAt = currentTimestamp,
            };
            await _orderItemRepository.Insert(entity);
            return new BaseResponse<bool>(true) { Success = true };
        }
        catch (Exception e)
        {
            Log.Logger.Error(e.Message, "Error creating order items");
            return new BaseResponse<bool> { Success = false, Message = e.Message };
        }
    }

    public async Task<BaseResponse<bool>> UpdateOrderItem(
        string orderItemId,
        UpdateOrderItemDto body
    )
    {
        try
        {
            var orderItem = await _orderItemRepository.GetById(orderItemId);
            if (orderItem == null)
                throw new Exception("Order item not found");
            orderItem.Quantity = body.Quantity;
            orderItem.UpdatedAt = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            await _orderItemRepository.Update(orderItem);
            return new BaseResponse<bool>(true) { Success = true };
        }
        catch (Exception e)
        {
            Log.Logger.Error(e.Message, "Error updating order items");
            return new BaseResponse<bool> { Success = false, Message = e.Message };
        }
    }

    public async Task<BaseResponse<bool>> DeleteOrderItem(string orderItemId)
    {
        try
        {
            await _orderItemRepository.Delete(orderItemId);
            return new BaseResponse<bool>(true) { Success = true };
        }
        catch (Exception e)
        {
            Log.Logger.Error(e.Message, "Error deleting order items");
            return new BaseResponse<bool> { Success = false, Message = e.Message };
        }
    }
}
