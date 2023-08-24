using ClientPricingSystem.Tests.Configuration;
using ClientPricingSystem.Core;
using Shouldly;
using MongoDB.Driver;
using ClientPricingSystem.Core.Documents;

namespace ClientPricingSystem.Tests.Vendor;
[TestCollection]
public class VendorServiceTests
{
    IMongoDatabase _context;
    public VendorServiceTests(IMongoDatabase context)
    {
        _context = context;
    }

    [TestMethod]
    public void TestTest()
    { 
        1.ShouldBe(1);
    }

    [TestMethod]
    public void AddVendorTest()
    {
        IMongoCollection<VendorDocument> vendorDocuments = _context.GetCollection<VendorDocument>(TestDatabase.Vendors);

        vendorDocuments.InsertOne(new VendorDocument { Name = "frogman", Domains = new List<string> { "youtube.com" }, Notes = "bestfriend" });

        vendorDocuments.Find(_ => true).CountDocuments().ShouldBe(4);
    }
}
