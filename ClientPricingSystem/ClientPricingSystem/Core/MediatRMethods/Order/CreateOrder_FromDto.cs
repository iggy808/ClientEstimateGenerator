using ClientPricingSystem.Configuration;
using ClientPricingSystem.Configuration.Mapper;
using ClientPricingSystem.Core.Documents;
using ClientPricingSystem.Core.Dtos;
using ClientPricingSystem.Core.MediatRMethods.Order.OrderItem;
using MediatR;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace ClientPricingSystem.Core.MediatRMethods.Order;
public class CreateOrder_FromDto
{
    public class Command : IRequest<Unit> 
    {
        public OrderDto OrderDto { get; set; }
    }

    public class Handler : IRequestHandler<Command, Unit>
    {
        IMongoDatabase _context;
        DatabaseConfiguration _config;
        IMediator _mediator;
        const decimal TAX = 400.00m;
        public Handler(IMongoClient mongoClient, IOptions<DatabaseConfiguration> config, IMediator mediator)
        {
            _context = mongoClient.GetDatabase(config.Value.DatabaseName);
            _config = config.Value;
            _mediator = mediator;
        }

        public async Task<Unit> Handle(Command command, CancellationToken cancellationToken)
        {
            IMongoCollection<ClientDocument> clientCollection = _context.GetCollection<ClientDocument>(_config.Clients);
            IMongoCollection<OrderDocument> orderCollection = _context.GetCollection<OrderDocument>(_config.Orders);

            OrderDocument order = OrderMapper.MapOrderDto_OrderDocument(command.OrderDto);

            // If the incoming request's Items field is populated, no json deserialization of order item input is required.
            //    => Map order items to the incoming request's items (Need to test this)
            if (command.OrderDto.Items != null)
                order.Items = command.OrderDto.Items;

            // Manually assign new ID (needed for mapping OrderItems documents to this order)
            order.Id = Guid.NewGuid();

            // Fetch and store client in OrderDocument
            order.Client = clientCollection.Find(Builders<ClientDocument>.Filter.Eq(c => c.Id, order.ClientId)).FirstOrDefault();

            decimal itemsTotal = 0.00m;
            if (order.Items != null)
            {
                // Create order items
                await _mediator.Send(new CreateOrderItems.Command { Items = order.Items, OrderId = order.Id }).ConfigureAwait(false);

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

            return Unit.Value;
        }
    }
}