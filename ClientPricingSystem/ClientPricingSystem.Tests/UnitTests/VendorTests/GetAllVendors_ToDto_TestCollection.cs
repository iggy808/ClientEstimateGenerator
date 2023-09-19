using ClientPricingSystem.Configuration;
using ClientPricingSystem.Configuration.Mapper;
using ClientPricingSystem.Core.Documents;
using ClientPricingSystem.Core.Dtos;
using ClientPricingSystem.Core.Methods.Vendor;
using ClientPricingSystem.Tests.Configuration;
using ClientPricingSystem.Tests.Fakers;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Moq;
using Shouldly;

namespace ClientPricingSystem.Tests.UnitTests.VendorTests;
[UnitTestCollection]
public class GetAllVendors_ToDto_TestCollection
{
    // Test constant variables
    const int TestVendorDatasetSize = 20;

    // Test setup variables
    IOptions<DatabaseConfiguration> _dbConfig;
    Mock<IMongoCollection<VendorDocument>> _vendorCollection;
    Mock<IMongoDatabase> _context;
    Mock<IMongoClient> _client;
    Mock<IAsyncCursor<VendorDocument>> _vendorCursor;

    // Test state variables
    List<VendorDocument> _testVendorDataset;

    #region Test Configuration Methods

    /* Setup Methods*/
    public void Setup()
    {
        SetupDbConfiguration();
    }

    void SetupDbConfiguration()
    {
        _dbConfig = Options.Create(new DatabaseConfiguration
        {
            DefaultConnectionString = TestDatabase.DefaultConnectionString,
            DatabaseName = TestDatabase.DatabaseName,
            Clients = TestDatabase.Clients,
            Vendors = TestDatabase.Vendors,
            Orders = TestDatabase.Orders,
            OrderItems = TestDatabase.OrderItems
        });
    }

    void SetupMocks()
    {
        _vendorCursor = new Mock<IAsyncCursor<VendorDocument>>();
        _vendorCursor.Setup(x => x.Current).Returns(_testVendorDataset);
        _vendorCursor.SetupSequence(x => x.MoveNext(It.IsAny<CancellationToken>())).Returns(true).Returns(false);
        _vendorCursor.SetupSequence(x => x.MoveNextAsync(It.IsAny<CancellationToken>())).Returns(Task.FromResult(true)).Returns(Task.FromResult(false));

        _vendorCollection = new Mock<IMongoCollection<VendorDocument>>();
        _vendorCollection.Setup(x =>
            x.FindAsync(
                It.IsAny<FilterDefinition<VendorDocument>>(),
                It.IsAny<FindOptions<VendorDocument, VendorDocument>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(_vendorCursor.Object);

        _context = new Mock<IMongoDatabase>();
        _context.Setup(x => x.GetCollection<VendorDocument>(It.IsAny<string>(), null)).Returns(_vendorCollection.Object);

        _client = new Mock<IMongoClient>();
        _client.Setup(x => x.GetDatabase(It.IsAny<string?>(), null)).Returns(_context.Object);
    }

    void SetupTestDataset()
    {
        _testVendorDataset = VendorFaker.GetVendorFaker().Generate(TestVendorDatasetSize);
    }

    #endregion

    #region Test Helper Methods

    void VerifyVendorMapperBehavior()
    {
        foreach (VendorDocument vendorDocument in _testVendorDataset)
        { 
            VendorDto mapperResult = VendorMapper.MapVendorDocument_VendorDto(vendorDocument);
            mapperResult.Id.ShouldBe(vendorDocument.Id);
            mapperResult.Name.ShouldBe(vendorDocument.Name);
            mapperResult.Notes.ShouldBe(vendorDocument.Notes);
            mapperResult.Domains.ShouldBe(vendorDocument.Domains);
        }
    }

    #endregion

    #region Method Tests

    [UnitTestMethod]
    public async Task SuccessfulWhen_TwentyVendors_InTestDataset()
    {
        // ARRANGE
        SetupTestDataset();
        SetupMocks();

        GetAllVendors_ToDto.Handler sut = new GetAllVendors_ToDto.Handler(_client.Object, _dbConfig);

        // ACT
        VendorDto? result = await sut.Handle(new GetAllVendors_ToDto.Query(), default);

        // ASSERT
        VerifyVendorMapperBehavior();

        result.ShouldNotBeNull();
        result.Vendors.Count.ShouldBe(_testVendorDataset.Count);
    }

    [UnitTestMethod]
    public async Task SuccessfulWhen_ZeroVendors_InTestDataset()
    {
        // ARRANGE
        _testVendorDataset = new List<VendorDocument>();
        SetupMocks();

        GetAllVendors_ToDto.Handler sut = new GetAllVendors_ToDto.Handler(_client.Object, _dbConfig);

        // ACT
        VendorDto? result = await sut.Handle(new GetAllVendors_ToDto.Query(), default);

        // ASSERT
        VerifyVendorMapperBehavior();

        result.ShouldNotBeNull();
        result.Vendors.Count.ShouldBe(_testVendorDataset.Count);
    }

    #endregion
}