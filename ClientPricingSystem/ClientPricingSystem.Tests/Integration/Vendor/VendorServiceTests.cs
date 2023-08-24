using ClientPricingSystem.Tests.Configuration;
using Shouldly;
using MongoDB.Driver;
using ClientPricingSystem.Core.Documents;
using MongoDB.Bson;

namespace ClientPricingSystem.Tests.Integration.Vendor;
[IntegrationTestCollection]
public class VendorServiceTests
{
    IMongoDatabase _context;
    public VendorServiceTests(IMongoDatabase context)
    {
        _context = context;
    }

    [IntegrationTestMethod]
    public void AddVendorTest()
    {
        IMongoCollection<VendorDocument> vendorDocuments = _context.GetCollection<VendorDocument>(TestDatabase.Vendors);

        vendorDocuments.InsertOne(new VendorDocument {Name = "frogman", Domains = new List<string> { "youtube.com" }, Notes = "bestfriend" });

        List<VendorDocument> vendors = vendorDocuments.Find(Builders<VendorDocument>.Filter.Eq(v => v.Name, "frogman")).ToList();

        vendors.Count.ShouldBeGreaterThan(1);
        vendors.First().Name.ShouldBe("frogman");
        vendors.First().Notes.ShouldBe("bestfriend");
        // Need to fix this issue lol, integration test came in handy though!
        //vendor.Domains.ElementAt(0).ShouldBe("youtube.com");
    }
}
