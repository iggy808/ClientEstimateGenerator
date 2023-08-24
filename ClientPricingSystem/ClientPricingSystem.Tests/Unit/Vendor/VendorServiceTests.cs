using ClientPricingSystem.Core.Documents;
using ClientPricingSystem.Tests.Configuration;
using Shouldly;

// NOTE: These are placeholder tests/collections

namespace ClientPricingSystem.Tests.Unit.Vendor;
[UnitTestCollection]
public class VendorServiceTests
{
    [UnitTestMethod]
    public void CreateVendorTest()
    {
        VendorDocument vendor = new VendorDocument { Name = "frogman", Domains = new List<string> { "youtube.com" }, Notes = "bestfriend" };
        vendor.Name.ShouldBe("frogman");
        vendor.Notes.ShouldBe("bestfriend");
        vendor.Domains.ElementAt(0).ShouldBe("youtube.com");
    }
}

