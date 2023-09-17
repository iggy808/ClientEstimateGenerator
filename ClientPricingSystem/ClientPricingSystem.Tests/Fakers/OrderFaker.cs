using Bogus;
using ClientPricingSystem.Core.Documents;

namespace ClientPricingSystem.Tests.Fakers;
public static class OrderFaker
{
    public static Faker<OrderDocument> GetOrderFaker(List<ClientDocument>? clients = null, List<VendorDocument> vendors = null, int TestClientRecordsCount = 1, int TestVendorRecordsCount = 1)
    {
        if (clients == null)
            clients = ClientFaker.GetClientFaker().Generate(TestClientRecordsCount);
        
        if (vendors == null)
            vendors = VendorFaker.GetVendorFaker().Generate(TestVendorRecordsCount);
        
        if (clients.Count == 1)
            return new Faker<OrderDocument>()
                .RuleFor(o => o.Id, f => f.Random.Uuid())
                .RuleFor(o => o.Client, f => clients.ElementAt(0))
                .RuleFor(o => o.ClientId, f => clients.ElementAt(0).Id)
                .RuleFor(o => o.ArtistFee, f => f.Random.Int(-1, 1) == 0 ? 5.00m : 125.00m)
                .RuleFor(o => o.Items, f => OrderItemFaker.GetOrderItemFaker(vendors).Generate(3));
        
        else
            return new Faker<OrderDocument>()
                .RuleFor(o => o.Id, f => f.Random.Uuid())
                .RuleFor(o => o.Client, f => clients.ElementAt(f.Random.Int(0, clients.Count - 1)))
                .RuleFor(o => o.ArtistFee, f => f.Random.Int(-1, 1) == 0 ? 5.00m : 125.00m)
                .RuleFor(o => o.Items, f => OrderItemFaker.GetOrderItemFaker(vendors).Generate(3));
    }
}