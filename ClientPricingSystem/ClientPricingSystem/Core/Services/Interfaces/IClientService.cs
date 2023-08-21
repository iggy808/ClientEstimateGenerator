using ClientPricingSystem.Core.Documents;

namespace ClientPricingSystem.Core.Services;
public interface IClientService
{
    public Task<List<ClientDocument>> GetAllClientsAsync();
    public Task CreateClientAsync(ClientDocument client);
    public Task<decimal> GetClientMarkupRate(Guid clientId);
}
