using ClientPricingSystem.Configuration;
using ClientPricingSystem.Configuration.Mapper;
using ClientPricingSystem.Core.Documents;
using ClientPricingSystem.Core.Dtos;
using MediatR;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace ClientPricingSystem.Core.Methods.Client;
public class CreateClient_FromDto
{
    public class Command : IRequest<Unit>
    {
        public ClientDto? ClientDto { get; set; }
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
            IMongoCollection<ClientDocument> clientCollection = _context.GetCollection<ClientDocument>(_config.Clients);

            ClientDocument client = ClientMapper.MapClientDto_ClientDocument(command.ClientDto);
            await clientCollection.InsertOneAsync(client).ConfigureAwait(false);

            return Unit.Value;
        }
    }
}

