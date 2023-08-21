using ClientPricingSystem.Configuration;
using ClientPricingSystem.Core.Documents;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace ClientPricingSystem.Core.Services;
public class ClientService : IClientService
{
    IMongoDatabase _context;
    FilterDefinitionBuilder<ClientDocument> _filterBuilder;
    DatabaseConfiguration _config;

    public ClientService(IMongoClient mongoClient, IOptions<DatabaseConfiguration> configuration)
    {
        _context = mongoClient.GetDatabase(configuration.Value.DatabaseName);
        _filterBuilder = Builders<ClientDocument>.Filter;
        _config = configuration.Value;
    }

    public async Task<List<ClientDocument>> GetAllClientsAsync()
    {
        IMongoCollection<ClientDocument> clients = _context.GetCollection<ClientDocument>(_config.Clients);
        return await clients.Find(_ => true).ToListAsync().ConfigureAwait(false);
    }

    public async Task<decimal> GetClientMarkupRate(Guid clientId)
    {
        IMongoCollection<ClientDocument> clients = _context.GetCollection<ClientDocument>(_config.Clients);
        FilterDefinition<ClientDocument> queryFilter = _filterBuilder.Eq(c => c.Id, clientId);

        List<ClientDocument> results = await clients.Find(queryFilter).ToListAsync().ConfigureAwait(false);

        if (results.Count != 0)
        {
            ClientDocument client = results.First();
            return client.MarkupRate;
        }
        return -1.0m;
    }

    public async Task CreateClientAsync(ClientDocument client)
    {
        IMongoCollection<ClientDocument> clients = _context.GetCollection<ClientDocument>(_config.Clients);
        await clients.InsertOneAsync(client).ConfigureAwait(false);
    }
}
