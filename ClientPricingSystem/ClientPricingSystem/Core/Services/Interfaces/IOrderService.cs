using ClientPricingSystem.Core.Documents;

namespace ClientPricingSystem.Core.Services;
public interface IOrderService
{
    public Task<List<OrderDocument>> GetAllOrdersAsync();

    public Task CreateOrderAsync(OrderDocument order);

    public Task CreateOrderItemsAsync(List<OrderItemDocument> orderItems, Guid? orderId);
}

