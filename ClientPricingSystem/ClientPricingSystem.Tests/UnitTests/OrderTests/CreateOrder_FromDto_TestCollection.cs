using Bogus;
using Bogus.DataSets;
using ClientPricingSystem.Configuration;
using ClientPricingSystem.Configuration.Mapper;
using ClientPricingSystem.Core.Documents;
using ClientPricingSystem.Core.Dtos;
using ClientPricingSystem.Core.MediatRMethods.Order;
using ClientPricingSystem.Core.MediatRMethods.Order.OrderItem;
using ClientPricingSystem.Tests.Configuration;
using ClientPricingSystem.Tests.Fakers;
using ClientPricingSystem.Tests.Helpers;
using MediatR;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Operations;
using Moq;
using Newtonsoft.Json;
using Shouldly;

namespace ClientPricingSystem.Tests.UnitTests.OrderTests;
[UnitTestCollection]
public class CreateOrder_FromDto_TestCollection
{
    const decimal TAX = 400.00m;
    // Test setup variables
    IOptions<DatabaseConfiguration> _dbConfig;
    Mock<IMongoCollection<OrderDocument>> _orderCollection;
    Mock<IAsyncCursor<ClientDocument>> _clientCursor;
    Mock<IMongoCollection<ClientDocument>> _clientCollection;
    Mock<IMongoDatabase> _context;
    Mock<IMongoClient> _client;
    Mock<IMediator> _mediator;

    List<ClientDocument> _testClientDataset;

    // Test state variables
    OrderDto _NewOrderDto;
    OrderDocument _NewOrder;
    List<OrderItemDocument> _NewOrderItems;

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
            .Callback((OrderDocument order, InsertOneOptions options, CancellationToken cancellationToken) => RecordNewlyCreatedOrder(order));        
        
        _clientCursor = new Mock<IAsyncCursor<ClientDocument>>();
        _clientCursor.Setup(x => x.Current).Returns(_testClientDataset);
        _clientCursor.SetupSequence(x => x.MoveNext(It.IsAny<CancellationToken>())).Returns(true).Returns(false);
        _clientCursor.SetupSequence(x => x.MoveNextAsync(It.IsAny<CancellationToken>())).Returns(Task.FromResult(true)).Returns(Task.FromResult(false));

        _clientCollection = new Mock<IMongoCollection<ClientDocument>>();
        _clientCollection.Setup(x =>
            x.FindSync(
                It.IsAny<FilterDefinition<ClientDocument>>(),
                It.IsAny<FindOptions<ClientDocument, ClientDocument>>(),
                It.IsAny<CancellationToken>()))
            .Returns(_clientCursor.Object);
        
        _context = new Mock<IMongoDatabase>();
        _context.Setup(x => x.GetCollection<OrderDocument>(It.IsAny<string>(), null)).Returns(_orderCollection.Object);
        _context.Setup(x => x.GetCollection<ClientDocument>(It.IsAny<string>(), null)).Returns(_clientCollection.Object);

        _client = new Mock<IMongoClient>();
        _client.Setup(x => x.GetDatabase(It.IsAny<string?>(), null)).Returns(_context.Object);

        _mediator = new Mock<IMediator>();
    }

    /* Callback Methods */
    void RecordNewlyCreatedOrder(OrderDocument order) 
    {
        _NewOrder = order;
    }

    #endregion

    #region Test Helper Methods

    ClientDocument GetClientFromId(Guid clientId)
    {
        return _testClientDataset.First(c => c.Id == clientId);
    }

    #endregion

    #region Method Tests
    
    [UnitTestMethod]
    public void SuccessfulWhen_ItemsJson_IsPopulatedAndValid()
    {
        // ARRANGE
        OrderDocument newOrder_preProcessing = OrderFaker.GetOrderFaker().Generate();
        OrderDto newOrderDto = new OrderDto
        {
            ClientId = newOrder_preProcessing.ClientId,
            ArtistFee = newOrder_preProcessing.ArtistFee,
            ItemsJson = JsonConvert.SerializeObject(newOrder_preProcessing.Items)
        };

        _NewOrderDto = newOrderDto;
        _testClientDataset = new List<ClientDocument> { newOrder_preProcessing.Client };

        SetupMocks();

        CreateOrder_FromDto.Handler sut = new CreateOrder_FromDto.Handler(_client.Object, _dbConfig, _mediator.Object);

        // ACT
        var result = sut.Handle(new CreateOrder_FromDto.Command { OrderDto = newOrderDto }, default);
        OrderDocument mapperResult = OrderMapper.MapOrderDto_OrderDocument(newOrderDto);

        // ASSERT
        // Verify the InsertOneAsync function was called exactly once
        _orderCollection.Verify(x =>
            x.InsertOneAsync(
                It.IsAny<OrderDocument>(),
                It.IsAny<InsertOneOptions>(),
                It.IsAny<CancellationToken>()),
            Times.Once());

        // Verify mapper logic worked appropriately with json
        decimal mapperResult_itemsTotalPrice = mapperResult.Items.Sum(i => i.ArticleQuantity * i.UnitPrice);

        mapperResult.ClientId.ShouldBe(newOrder_preProcessing.ClientId);
        mapperResult.ArtistFee.ShouldBe(newOrder_preProcessing.ArtistFee);
        mapperResult.Items.ShouldNotBeNull().ShouldNotBeEmpty();
        mapperResult_itemsTotalPrice.ShouldBe(newOrder_preProcessing.Items.Sum(i => i.ArticleQuantity * i.UnitPrice));
        mapperResult_itemsTotalPrice.ShouldBeGreaterThan(0.00m);

        _NewOrder.SubTotal.ShouldBe(newOrder_preProcessing.Items.Sum(i => i.ArticleQuantity * i.UnitPrice) + newOrder_preProcessing.ArtistFee + newOrder_preProcessing.Client.MarkupRate);
        _NewOrder.Total.ShouldBe(_NewOrder.SubTotal + TAX);
    }

    #endregion
}