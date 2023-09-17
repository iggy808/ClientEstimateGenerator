using Bogus;
using ClientPricingSystem.Core.Documents;

namespace ClientPricingSystem.Tests.Fakers;
public static class ClientFaker
{
    public static Faker<ClientDocument> GetClientFaker()
    {
        return new Faker<ClientDocument>()
            .RuleFor(c => c.Id, f => f.Random.Uuid())
            .RuleFor(c => c.Name, f => f.Company.CompanyName())
            .RuleFor(c => c.MarkupRate, f => Math.Round(f.Random.Decimal(0.00m, 125.00m), 2))
            .RuleFor(c => c.Address, f => f.Address.FullAddress());
    }
}