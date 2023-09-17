using ClientPricingSystem.Configuration;
using ClientPricingSystem.Core.Documents;
using ClientPricingSystem.Core.Dtos;
using ClientPricingSystem.Core.MediatRMethods.Client;
using ClientPricingSystem.Core.Validators.Client;
using ClientPricingSystem.Tests.Configuration;
using ClientPricingSystem.Tests.Fakers;
using FluentValidation.Results;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.Core.Operations;
using Moq;
using Shouldly;
using System.Linq.Expressions;

namespace ClientPricingSystem.Tests.UnitTests.ClientTests;
[UnitTestCollection]
public class GetAllClients_ToDto_TestCollection
{
    // Test constant variables
    const int TestClientDatasetSize = 20;

    // Test setup variables
    IOptions<DatabaseConfiguration> _dbConfig;
    Mock<IMongoCollection<ClientDocument>> _clientCollection;
    Mock<IMongoDatabase> _context;
    Mock<IMongoClient> _client;
    Mock<IAsyncCursor<ProjectionDefinition<ClientDocument>>> _asyncCursor;

    // Test state variables
    List<ClientDocument> _TestClientDataset;

    #region Test Configuration Methods

    /* Setup Methods*/
    void SetupTestDataset()
    {
        _TestClientDataset = ClientFaker.GetClientFaker().Generate(TestClientDatasetSize);
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

        //NOTE:
        //    Cannot Mock queries currently
        //    - Current query implementations are not suitable for mocking
        //    - Bookmarked page with potential solution in desktop browser
        //      - Will need to rework Find calls to be FindSync calls in order to mock effectively for unit tests
        //      - I believe defining query filters this way will be a better practice in general as well

        // Below ripped from stack overflow, only partially implemented, need to do a lot more reserach here
        _asyncCursor = new Mock<IAsyncCursor<ProjectionDefinition<ClientDocument>>>();

        _clientCollection = new Mock<IMongoCollection<ClientDocument>>();
        _clientCollection.Setup(x => 
            x.FindSync(
                It.IsAny<FilterDefinition<ClientDocument>>(),
                It.IsAny<FindOptions<ClientDocument, ProjectionDefinition<ClientDocument>>>(),
                It.IsAny<CancellationToken>()))
            .Returns(_asyncCursor.Object);

        _context = new Mock<IMongoDatabase>();
        _context.Setup(x => x.GetCollection<ClientDocument>(It.IsAny<string>(), null)).Returns(_clientCollection.Object);

        _client = new Mock<IMongoClient>();
        _client.Setup(x => x.GetDatabase(It.IsAny<string?>(), null)).Returns(_context.Object);
    }

    #endregion

    #region Method Tests

    [UnitTestMethod]
    public void SuccessfulWhen_AllDtoFieldsPopulated()
    {
        // ARRANGE
        SetupTestDataset();
        SetupMocks();

        ClientDocument newClient = ClientFaker.GetClientFaker().Generate();
        ClientDto newClientDto = new ClientDto
        { 
            Name = newClient.Name,
            Address = newClient.Address,
            MarkupRate = newClient.MarkupRate
        };

        GetAllCleints_ToDto.Handler sut = new GetAllCleints_ToDto.Handler(_client.Object, _dbConfig);

        // ACT
        var result = sut.Handle(new GetAllCleints_ToDto.Query(), default);

        // ASSERT
        // Verify the Find function was called exactly once
        _clientCollection.Verify(x =>
            x.Find(
                It.IsAny<Expression<Func<ClientDocument, bool>>>(),
                It.IsAny<FindOptions>())
            .ToListAsync(It.IsAny<CancellationToken>()),
            Times.Once());

    }

    [UnitTestMethod]
    public void SuccessfulWhen_RequiredDtoFieldsPopulated()
    {
        // ARRANGE
        SetupTestDataset();
        SetupMocks();

        ClientDocument newClient = ClientFaker.GetClientFaker().Generate();
        ClientDto newClientDto = new ClientDto
        {
            Name = newClient.Name,
            MarkupRate = newClient.MarkupRate
        };

        CreateClient_FromDto.Handler sut = new CreateClient_FromDto.Handler(_client.Object, _dbConfig);

        // ACT
        var result = sut.Handle(new CreateClient_FromDto.Command { ClientDto = newClientDto }, default);

        // ASSERT
        // Verify the InsertOneAsync function was called exactly once
        _clientCollection.Verify(x =>
            x.InsertOneAsync(
                It.IsAny<ClientDocument>(),
                It.IsAny<InsertOneOptions>(),
                It.IsAny<CancellationToken>()),
            Times.Once());

        // Verify mapper appropriately mapped the ClientDto object to the ClientDocument object
        //_DtoToDocument_MapperResult.Name.ShouldBe(newClientDto.Name);
        //_DtoToDocument_MapperResult.MarkupRate.ShouldBe(newClientDto.MarkupRate);
        //_DtoToDocument_MapperResult.Address.ShouldBeNull();
        1.ShouldBe(2);
    }

    #endregion
}