using ClientPricingSystem.Configuration;
using ClientPricingSystem.Core.Documents;
using MediatR;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace ClientPricingSystem.Core.MediatRMethods.Order;
public class CreateOrderItems
{
    public class Command : IRequest<Unit> 
    {
        public List<OrderItemDocument> Items { get; set; }
        public Guid? OrderId { get; set; }
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
            IMongoCollection<OrderItemDocument> orderItemCollection = _context.GetCollection<OrderItemDocument>(_config.OrderItems);

            if (command.OrderId != null)
            {
                command.Items.ForEach(i => i.OrderId = command.OrderId.Value);
            }

            await orderItemCollection.InsertManyAsync(command.Items).ConfigureAwait(false);

            return Unit.Value;
        }
    }
}