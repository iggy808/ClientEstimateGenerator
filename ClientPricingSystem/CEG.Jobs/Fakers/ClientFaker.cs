using Bogus;
using ClientPricingSystem.Core.Documents;

namespace CEG.Jobs.Fakers;
public static class ClientFaker
{
    public static Faker<ClientDocument> GetClientFaker()
    {
        return new Faker<ClientDocument>()
            .RuleFor(c => c.Name, f => f.Company.CompanyName())
            .RuleFor(c => c.MarkupRate, f => f.Random.Decimal(0.00m, 125.00m))
            .RuleFor(c => c.Address, f => f.Address.FullAddress());
    }
}

