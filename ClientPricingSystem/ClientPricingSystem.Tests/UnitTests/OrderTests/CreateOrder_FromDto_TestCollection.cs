using ClientPricingSystem.Configuration;
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
public class CreateOrder_FromDto_TestCollection
{
    // Test setup variables
    IOptions<DatabaseConfiguration> _dbConfig;
    Mock<IMongoCollection<OrderDocument>> _orderCollection;
    Mock<IMongoDatabase> _context;
    Mock<IMongoClient> _client;

    // Test state variables
    OrderDocument _DtoToDocument_MapperResult;

    #region Test Configuration Methods

    /* Setup Methods*/
    void SetupMocks()
    {
        _dbConfig = Options.Create(new DatabaseConfiguration
        {
            DefaultConnectionString = TestDatabase.DefaultConnectionString,
            DatabaseName = TestDatabase.DatabaseName,
            Clients = TestDatabase.Clients,
            Orders = TestDatabase.Orders,
            Vendors = TestDatabase.Vendors,
            OrderItems = TestDatabase.OrderItems
        });

        _orderCollection = new Mock<IMongoCollection<OrderDocument>>();
        // Setup callback to record result of mapping the OrderDto object to the OrderDocument object
        _orderCollection.Setup(x =>
            x.InsertOneAsync(
                It.IsAny<OrderDocument>(),
                It.IsAny<InsertOneOptions>(),
                It.IsAny<CancellationToken>()))
            .Callback((OrderDocument order, InsertOneOptions options, CancellationToken cancellationToken) => Record_DtoToDocument_MapperResult(order));

        _context = new Mock<IMongoDatabase>();
        _context.Setup(x => x.GetCollection<OrderDocument>(It.IsAny<string>(), null)).Returns(_orderCollection.Object);

        _client = new Mock<IMongoClient>();
        _client.Setup(x => x.GetDatabase(It.IsAny<string?>(), null)).Returns(_context.Object);
    }

    /* Callback Methods */
    void Record_DtoToDocument_MapperResult(OrderDocument order) 
    {
        _DtoToDocument_MapperResult = order;
    }

    #endregion

    #region Method Tests
    /* Will come back to this
    [UnitTestMethod]
    public void SuccessfulWhen_AllDtoFieldsPopulated()
    {
        // ARRANGE
        SetupMocks();

        OrderDocument newOrder = OrderFaker.GetOrderFaker().Generate();
        OrderDto newOrderDto = new OrderDto();

        CreateOrder_FromDto.Handler sut = new CreateOrder_FromDto.Handler(_client.Object, _dbConfig);

        // ACT
        var result = sut.Handle(new CreateOrder_FromDto.Command{ OrderDto = newOrderDto }, default);

        // ASSERT
        // Verify the InsertOneAsync function was called exactly once
        _orderCollection.Verify(x =>
            x.InsertOneAsync(
                It.IsAny<OrderDocument>(),
                It.IsAny<InsertOneOptions>(),
                It.IsAny<CancellationToken>()),
            Times.Once());

        // Verify mapper appropriately mapped the OrderDto object to the OrderDocument object
        _DtoToDocument_MapperResult.Name.ShouldBe(newOrderDto.Name);
        _DtoToDocument_MapperResult.Domains.ShouldBe(newOrderDto.Domains);
        _DtoToDocument_MapperResult.Notes.ShouldBe(newOrderDto.Notes);
    }

    [UnitTestMethod]
    public void SuccessfulWhen_RequiredDtoFieldsPopulated()
    {
        // ARRANGE
        SetupMocks();

        OrderDocument newOrder = OrderFaker.GetOrderFaker().Generate();
        OrderDto newOrderDto = new OrderDto
        {
            Name = newOrder.Name
        };

        CreateOrder_FromDto.Handler sut = new CreateOrder_FromDto.Handler(_client.Object, _dbConfig);

        // ACT
        var result = sut.Handle(new CreateOrder_FromDto.Command { OrderDto = newOrderDto }, default);

        // ASSERT
        // Verify the InsertOneAsync function was called exactly once
        _orderCollection.Verify(x =>
            x.InsertOneAsync(
                It.IsAny<OrderDocument>(),
                It.IsAny<InsertOneOptions>(),
                It.IsAny<CancellationToken>()),
            Times.Once());

        // Verify mapper appropriately mapped the OrderDto object to the OrderDocument object
        _DtoToDocument_MapperResult.Name.ShouldBe(newOrderDto.Name);
        _DtoToDocument_MapperResult.Domains.ShouldBeNull();
        _DtoToDocument_MapperResult.Notes.ShouldBeNull();
    }

    [UnitTestMethod]
    public void SuccessfulWhen_DomainsRaw_IsNotNullOrEmpty()
    {
        // ARRANGE
        SetupMocks();

        OrderDocument newOrder = OrderFaker.GetOrderFaker().Generate();
        OrderDto newOrderDto = new OrderDto
        {
            Name = newOrder.Name,
            DomainsRaw = string.Join(", ", newOrder.Domains)
        };

        CreateOrder_FromDto.Handler sut = new CreateOrder_FromDto.Handler(_client.Object, _dbConfig);

        // ACT
        var result = sut.Handle(new CreateOrder_FromDto.Command { OrderDto = newOrderDto }, default);

        // ASSERT
        // Verify the InsertOneAsync function was called exactly once
        _orderCollection.Verify(x =>
            x.InsertOneAsync(
                It.IsAny<OrderDocument>(),
                It.IsAny<InsertOneOptions>(),
                It.IsAny<CancellationToken>()),
            Times.Once());

        // Verify mapper appropriately mapped the OrderDto object to the OrderDocument object
        _DtoToDocument_MapperResult.Name.ShouldBe(newOrderDto.Name);
        _DtoToDocument_MapperResult.Domains.ShouldBe(newOrder.Domains);
        _DtoToDocument_MapperResult.Notes.ShouldBeNull();
    }
    */
    #endregion
}