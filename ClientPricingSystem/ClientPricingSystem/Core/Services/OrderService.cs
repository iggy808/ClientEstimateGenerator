using ClientPricingSystem.Configuration;
using ClientPricingSystem.Core.Documents;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace ClientPricingSystem.Core.Services;

public class OrderService : IOrderService
{
    IMongoDatabase _context;
    DatabaseConfiguration _config;

    const decimal TAX = 400.00m;

    public OrderService(IMongoClient mongoClient, IOptions<DatabaseConfiguration> config)
    {
        _context = mongoClient.GetDatabase(config.Value.DatabaseName);
        _config = config.Value;
    }

    public async Task<List<OrderDocument>> GetAllOrdersAsync()
    {
        IMongoCollection<OrderDocument> orders = _context.GetCollection<OrderDocument>(_config.Orders);        
        return await orders.Find(_ => true).ToListAsync().ConfigureAwait(false);
    }

    public async Task CreateOrderAsync(OrderDocument order)
    {
        IMongoCollection<OrderDocument> orderCollection = _context.GetCollection<OrderDocument>(_config.Orders);
        IMongoCollection<ClientDocument> clientCollection = _context.GetCollection<ClientDocument>(_config.Clients);

        // Manually assign new ID (needed for mapping OrderItems documents to this order)
        order.Id = Guid.NewGuid();

        // Fetch and store client in OrderDocument
        order.Client = clientCollection.Find(Builders<ClientDocument>.Filter.Eq(c => c.Id, order.ClientId)).FirstOrDefault();

        decimal itemsTotal = 0.00m;
        if (order.Items != null)
        {
            // Create order items
            await CreateOrderItemsAsync(order.Items, order.Id).ConfigureAwait(false);

            // Calculate order totals
            foreach (OrderItemDocument item in order.Items)
            {
                // Need to determine how to calculate totals from business
                //itemsTotal += item.ArticleQuantity * item.UnitPrice;
                itemsTotal += item.Total;
            }
        }

        order.SubTotal = itemsTotal + order.Client.MarkupRate + order.ArtistFee;
        order.Total = order.SubTotal + TAX;

        await orderCollection.InsertOneAsync(order).ConfigureAwait(false);
    }

    public async Task CreateOrderItemsAsync(List<OrderItemDocument> orderItems, Guid? orderId = null)
    {
        IMongoCollection<OrderItemDocument> orderItemCollection = _context.GetCollection<OrderItemDocument>(_config.OrderItems);
        if (orderId != null)
        {
            orderItems.ForEach(i => i.OrderId = orderId.Value);
        }
        await orderItemCollection.InsertManyAsync(orderItems).ConfigureAwait(false);
    }
}