using ClientPricingSystem.Configuration;
using ClientPricingSystem.Configuration.Mapper;
using ClientPricingSystem.Core.Documents;
using ClientPricingSystem.Core.Dtos;
using MediatR;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace ClientPricingSystem.Core.Methods.Order;
public class GetAllOrders_ToDto
{
    public class Query : IRequest<OrderDto> { }

    public class Handler : IRequestHandler<Query, OrderDto>
    {
        IMongoDatabase _context;
        DatabaseConfiguration _config;
        public Handler(IMongoClient mongoClient, IOptions<DatabaseConfiguration> config)
        {
            _context = mongoClient.GetDatabase(config.Value.DatabaseName);
            _config = config.Value;
        }

        public async Task<OrderDto> Handle(Query query, CancellationToken cancellationToken)
        {
            IMongoCollection<OrderDocument> orderCollection = _context.GetCollection<OrderDocument>(_config.Orders);
            List<OrderDocument> orders = await orderCollection.Find(Builders<OrderDocument>.Filter.Exists(c => c.Id)).ToListAsync().ConfigureAwait(false);

            OrderDto orderDto = new OrderDto
            {
                Orders = orders.Select(o => OrderMapper.MapOrderDocument_OrderDto(o)).ToList()
            };

            return orderDto;
        }
    }
}