﻿using Bogus;
using ClientPricingSystem.Core.Documents;

namespace ClientPricingSystem.Tests.Fakers;
public static class VendorFaker
{
    public static Faker<VendorDocument> GetVendorFaker()
    {
        return new Faker<VendorDocument>()
            .RuleFor(v => v.Id, f => f.Random.Uuid())
            .RuleFor(v => v.Name, f => f.Company.CompanyName())
            .RuleFor(v => v.Domains, f => new List<string> { f.Internet.Url(), f.Internet.Url() })
            .RuleFor(v => v.Notes, f => String.Join(" ", f.Lorem.Words(7)));
    }
}