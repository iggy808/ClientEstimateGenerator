using Bogus;
using ClientPricingSystem.Configuration;
using ClientPricingSystem.Core.Documents;
using ClientPricingSystem.Core.MediatRMethods.Order.OrderItem;
using ClientPricingSystem.Core.Validators.Order.OrderItem;
using ClientPricingSystem.Tests.Configuration;
using ClientPricingSystem.Tests.Fakers;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Moq;
using Shouldly;

namespace ClientPricingSystem.Tests.UnitTests.OrderTests;
[UnitTestCollection]
public class CreateOrderItems_TestCollection
{
    // Test setup variables
    IOptions<DatabaseConfiguration> _dbConfig;
    Mock<IMongoCollection<OrderItemDocument>> _orderCollection;
    Mock<IMongoDatabase> _context;
    Mock<IMongoClient> _client;

    // Test state variables
    OrderDocument _BaseOrder;
    IEnumerable<OrderItemDocument> _UpdatedOrderItems;

    #region Test Configuration Methods

    /* Setup Methods*/
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

        _orderCollection = new Mock<IMongoCollection<OrderItemDocument>>();
        _orderCollection.Setup(x =>
            x.InsertManyAsync(
                It.IsAny<IEnumerable<OrderItemDocument>>(),
                It.IsAny<InsertManyOptions>(),
                It.IsAny<CancellationToken>()))
            .Callback((IEnumerable<OrderItemDocument> items, InsertManyOptions options, CancellationToken cancellationToken) =>
            {
                RecordUpdatedOrderItems(items);
            });

        _context = new Mock<IMongoDatabase>();
        _context.Setup(x => x.GetCollection<OrderItemDocument>(It.IsAny<string>(), null)).Returns(_orderCollection.Object);

        _client = new Mock<IMongoClient>();
        _client.Setup(x => x.GetDatabase(It.IsAny<string?>(), null)).Returns(_context.Object);
    }

    #endregion

    #region Test Helper Methods

    void RecordUpdatedOrderItems(IEnumerable<OrderItemDocument> items)
    { 
        _UpdatedOrderItems = items;
    }

    bool ValidateTestCommand(CreateOrderItems.Command command)
    {
        CreateOrderItems_CommandValidator validator = new CreateOrderItems_CommandValidator();
        FluentValidation.Results.ValidationResult validationResult = validator.Validate(command);
        return validationResult.IsValid;
    }

    void VerifyOrderItemsHaveAppropriateOrderId()
    {
        foreach (OrderItemDocument item in _UpdatedOrderItems)
        {
            item.OrderId.ShouldBe(_BaseOrder.Id);
        }
    }

    #endregion

    #region Method Tests

    [UnitTestMethod]
    public async Task SuccessfulWhen_ThreeItems_InOrder()
    {
        // ARRANGE
        SetupMocks();

        OrderDocument newOrder = OrderFaker.GetOrderFaker().Generate();
        newOrder.Items.ForEach(oi => oi.OrderId = Guid.Empty);
        _BaseOrder = newOrder;

        CreateOrderItems.Handler sut = new CreateOrderItems.Handler(_client.Object, _dbConfig);

        CreateOrderItems.Command command = new CreateOrderItems.Command { Order = newOrder };

        if (!ValidateTestCommand(command))
            throw new ValidationException("Test command is not valid.");

        // ACT
        await sut.Handle(command, default);

        // ASSERT
        VerifyOrderItemsHaveAppropriateOrderId();
    }

    [UnitTestMethod]
    public async Task SuccessfulWhen_OneItem_InOrder()
    {
        // ARRANGE
        SetupMocks();

        OrderDocument newOrder = OrderFaker.GetOrderFaker().Generate();
        newOrder.Items.ForEach(oi => oi.OrderId = Guid.Empty);
        _BaseOrder = newOrder;

        CreateOrderItems.Handler sut = new CreateOrderItems.Handler(_client.Object, _dbConfig);

        CreateOrderItems.Command command = new CreateOrderItems.Command { Order = newOrder };

        if (!ValidateTestCommand(command))
            throw new ValidationException("Test command is not valid.");

        // ACT
        await sut.Handle(command, default);

        // ASSERT
        VerifyOrderItemsHaveAppropriateOrderId();
    }

    #endregion
}