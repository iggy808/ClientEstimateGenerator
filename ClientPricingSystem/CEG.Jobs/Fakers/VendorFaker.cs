using Bogus;
using ClientPricingSystem.Core.Documents;

namespace CEG.Jobs.Fakers;
public static class VendorFaker
{
    public static Faker<VendorDocument> GetVendorFaker()
    {
        return new Faker<VendorDocument>()
            .RuleFor(v => v.Name, f => f.Company.CompanyName())
            .RuleFor(v => v.Domains, f => new List<string> { f.Internet.Url() })
            .RuleFor(v => v.Notes, f => f.Lorem.Words(7).ToString());
    }
}

