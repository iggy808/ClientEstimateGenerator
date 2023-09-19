using ClientPricingSystem.Configuration;
using ClientPricingSystem.Core.Documents;
using MediatR;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace ClientPricingSystem.Core.Methods.Order.OrderItem;
public class CreateOrderItems
{
    public class Command : IRequest<Unit>
    {
        public OrderDocument Order { get; set; }
    }

    public class Handler : IRequestHandler<Command, Unit>
    {
        IMongoDatabase _context;
        DatabaseConfiguration _config;
        public Handler(IMongoClient mongoClient, IOptions<DatabaseConfiguration> config)
        {
            _context = mongoClient.GetDatabase(config.Value.DatabaseName);
            _config = config.Value;
        }

        public async Task<Unit> Handle(Command command, CancellationToken cancellationToken)
        {
            command.Order.Items.ForEach(i => i.OrderId = command.Order.Id);

            IMongoCollection<OrderItemDocument> orderItemCollection = _context.GetCollection<OrderItemDocument>(_config.OrderItems);
            await orderItemCollection.InsertManyAsync(command.Order.Items).ConfigureAwait(false);

            return Unit.Value;
        }
    }
}