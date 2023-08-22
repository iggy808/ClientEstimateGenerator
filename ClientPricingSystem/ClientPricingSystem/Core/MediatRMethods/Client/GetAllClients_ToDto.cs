using ClientPricingSystem.Configuration;
using ClientPricingSystem.Configuration.Mapper;
using ClientPricingSystem.Core.Documents;
using ClientPricingSystem.Core.Dtos;
using MediatR;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace ClientPricingSystem.Core.MediatRMethods.Client;
public class GetAllCleints_ToDto
{
    public class Query : IRequest<ClientDto> { }

    public class Handler : IRequestHandler<Query, ClientDto>
    {
        IMongoDatabase _context;
        DatabaseConfiguration _config;
        public Handler(IMongoClient mongoClient, IOptions<DatabaseConfiguration> config)
        {
            _context = mongoClient.GetDatabase(config.Value.DatabaseName);
            _config = config.Value;
        }

        public async Task<ClientDto> Handle(Query query, CancellationToken cancellationToken)
        {
            IMongoCollection<ClientDocument> clientCollection = _context.GetCollection<ClientDocument>(_config.Clients);
            List<ClientDocument> clients = await clientCollection.Find(_ => true).ToListAsync().ConfigureAwait(false);

            ClientDto clientDto = new ClientDto
            {
                Clients = clients.Select(c => ClientMapper.MapClientDocument_ClientDto(c)).ToList()
            };

            return clientDto;
        }
    }
}