using ClientPricingSystem.Configuration;
using ClientPricingSystem.Configuration.Mapper;
using ClientPricingSystem.Core.Documents;
using ClientPricingSystem.Core.Dtos;
using MediatR;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace ClientPricingSystem.Core.MediatRMethods.Order;
public class CreateOrderItems
{
    public class Query : IRequest<Unit> 
    {
        public List<OrderItemDocument> Items { get; set; }
        public Guid? OrderId { get; set; }
    }

    public class Handler : IRequestHandler<Query, Unit>
    {
        IMongoDatabase _context;
        DatabaseConfiguration _config;
        public Handler(IMongoClient mongoClient, IOptions<DatabaseConfiguration> config)
        {
            _context = mongoClient.GetDatabase(config.Value.DatabaseName);
            _config = config.Value;
        }

        public async Task<Unit> Handle(Query query, CancellationToken cancellationToken)
        {
            IMongoCollection<OrderItemDocument> orderItemCollection = _context.GetCollection<OrderItemDocument>(_config.OrderItems);

            if (query.OrderId != null)
            {
                query.Items.ForEach(i => i.OrderId = query.OrderId.Value);
            }

            await orderItemCollection.InsertManyAsync(query.Items).ConfigureAwait(false);

            return Unit.Value;
        }
    }
}