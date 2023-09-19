using ClientPricingSystem.Configuration;
using ClientPricingSystem.Configuration.Mapper;
using ClientPricingSystem.Core.Documents;
using ClientPricingSystem.Core.Dtos;
using ClientPricingSystem.Core.MediatRMethods.Order;
using ClientPricingSystem.Tests.Configuration;
using ClientPricingSystem.Tests.Fakers;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Moq;
using Shouldly;

namespace ClientPricingSystem.Tests.UnitTests.OrderTests;
[UnitTestCollection]
public class GetAllOrders_ToDto_TestCollection
{
    // Test constant variables
    const int TestOrderDatasetSize = 20;

    // Test setup variables
    IOptions<DatabaseConfiguration> _dbConfig;
    Mock<IMongoCollection<OrderDocument>> _orderCollection;
    Mock<IMongoDatabase> _context;
    Mock<IMongoClient> _client;
    Mock<IAsyncCursor<OrderDocument>> _orderCursor;

    // Test state variables
    List<OrderDocument> _testOrderDataset;

    #region Test Configuration Methods

    /* Setup Methods*/
    void SetupTestDataset()
    {
        _testOrderDataset = OrderFaker.GetOrderFaker().Generate(TestOrderDatasetSize);
        // Items not stored alongside order in database, this line mimicks this
        _testOrderDataset.ForEach(o => o.Items = null);
    }

    void SetupMocks()
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

        _orderCursor = new Mock<IAsyncCursor<OrderDocument>>();
        _orderCursor.Setup(x => x.Current).Returns(_testOrderDataset);
        _orderCursor.SetupSequence(x => x.MoveNext(It.IsAny<CancellationToken>())).Returns(true).Returns(false);
        _orderCursor.SetupSequence(x => x.MoveNextAsync(It.IsAny<CancellationToken>())).Returns(Task.FromResult(true)).Returns(Task.FromResult(false));

        _orderCollection = new Mock<IMongoCollection<OrderDocument>>();
        _orderCollection.Setup(x =>
            x.FindAsync(
                It.IsAny<FilterDefinition<OrderDocument>>(),
                It.IsAny<FindOptions<OrderDocument, OrderDocument>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(_orderCursor.Object);

        _context = new Mock<IMongoDatabase>();
        _context.Setup(x => x.GetCollection<OrderDocument>(It.IsAny<string>(), null)).Returns(_orderCollection.Object);

        _client = new Mock<IMongoClient>();
        _client.Setup(x => x.GetDatabase(It.IsAny<string?>(), null)).Returns(_context.Object);
    }

    #endregion

    #region Test Helper Methods

    void VerifyOrderMapperBehavior()
    {
        foreach (OrderDocument orderDocument in _testOrderDataset)
        { 
            OrderDto mapperResult = OrderMapper.MapOrderDocument_OrderDto(orderDocument);
            mapperResult.Id.ShouldBe(orderDocument.Id);
            mapperResult.ClientId.ShouldBe(orderDocument.ClientId);
            mapperResult.ArtistFee.ShouldBe(orderDocument.ArtistFee);
            mapperResult.SubTotal.ShouldBe(orderDocument.SubTotal);
            mapperResult.Total.ShouldBe(orderDocument.Total);
        }
    }

    #endregion

    #region Method Tests

    [UnitTestMethod]
    public async Task SuccessfulWhen_TwentyOrders_InTestDataset()
    {
        // ARRANGE
        SetupTestDataset();
        SetupMocks();

        GetAllOrders_ToDto.Handler sut = new GetAllOrders_ToDto.Handler(_client.Object, _dbConfig);

        // ACT
        OrderDto? result = await sut.Handle(new GetAllOrders_ToDto.Query(), default);

        // ASSERT
        VerifyOrderMapperBehavior();

        result.ShouldNotBeNull();
        result.Orders.Count.ShouldBe(_testOrderDataset.Count);
    }

    [UnitTestMethod]
    public async Task SuccessfulWhen_ZeroOrders_InTestDataset()
    {
        // ARRANGE
        _testOrderDataset = new List<OrderDocument>();
        SetupMocks();

        GetAllOrders_ToDto.Handler sut = new GetAllOrders_ToDto.Handler(_client.Object, _dbConfig);

        // ACT
        OrderDto? result = await sut.Handle(new GetAllOrders_ToDto.Query(), default);

        // ASSERT
        VerifyOrderMapperBehavior();

        result.ShouldNotBeNull();
        result.Orders.Count.ShouldBe(_testOrderDataset.Count);
    }

    #endregion
}