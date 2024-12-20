using me.admin.api.Data.Repositories;

namespace me.admin.api.Services;

public class OrderService(OrderRepository orderRepository)
{
	readonly OrderRepository _orderRepository = orderRepository;
}