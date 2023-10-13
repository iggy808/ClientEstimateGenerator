using Bogus;
using ClientPricingSystem.Core.Documents;
using ClientPricingSystem.Core.Enums;

namespace ClientPricingSystem.Tests.Fakers;

public static class OrderItemFaker
{
    public static Faker<OrderItemDocument> GetOrderItemFaker(List<VendorDocument> vendors = null)
    {
        if (vendors == null)
        {
            vendors = VendorFaker.GetVendorFaker().Generate(3);
        }

        List<Size> sizes = Enum.GetValues<Size>().ToList();

        return new Faker<OrderItemDocument>()
            .RuleFor(i => i.Id, f => f.Random.Uuid())
            .RuleFor(i => i.ArticleQuantity, f => f.Random.Int(1, 17) * 12)
            .RuleFor(i => i.UnitPrice, f => Math.Round(f.Random.Decimal(0.25m, 2.66m), 3))
            .RuleFor(i => i.Size, f => sizes.ElementAt(f.Random.Int(0, sizes.Count - 1)))
            .RuleFor(i => i.VendorId, f => vendors.ElementAt(f.Random.Int(0, vendors.Count - 1)).Id);
    }
}