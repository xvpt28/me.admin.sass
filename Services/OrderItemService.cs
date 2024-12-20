using me.admin.api.Data.Repositories;

namespace me.admin.api.Services;

public class OrderItemService(OrderItemRepository orderItemRepository)
{
	readonly OrderItemRepository _orderItemRepository = orderItemRepository;
}