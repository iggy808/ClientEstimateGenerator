using ClientPricingSystem.Configuration;
using ClientPricingSystem.Configuration.Mapper;
using ClientPricingSystem.Core.Documents;
using ClientPricingSystem.Core.Dtos;
using ClientPricingSystem.Core.Methods.Client;
using ClientPricingSystem.Tests.Configuration;
using ClientPricingSystem.Tests.Fakers;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Moq;
using Shouldly;

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
    Mock<IAsyncCursor<ClientDocument>> _clientCursor;

    // Test state variables
    List<ClientDocument> _testClientDataset;

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

    void SetupTestDataset()
    {
        _testClientDataset = ClientFaker.GetClientFaker().Generate(TestClientDatasetSize);
    }

    void SetupMocks()
    {
        _clientCursor = new Mock<IAsyncCursor<ClientDocument>>();
        _clientCursor.Setup(x => x.Current).Returns(_testClientDataset);
        _clientCursor.SetupSequence(x => x.MoveNext(It.IsAny<CancellationToken>())).Returns(true).Returns(false);
        _clientCursor.SetupSequence(x => x.MoveNextAsync(It.IsAny<CancellationToken>())).Returns(Task.FromResult(true)).Returns(Task.FromResult(false));

        _clientCollection = new Mock<IMongoCollection<ClientDocument>>();
        _clientCollection.Setup(x =>
            x.FindAsync(
                It.IsAny<FilterDefinition<ClientDocument>>(),
                It.IsAny<FindOptions<ClientDocument, ClientDocument>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(_clientCursor.Object);

        _context = new Mock<IMongoDatabase>();
        _context.Setup(x => x.GetCollection<ClientDocument>(It.IsAny<string>(), null)).Returns(_clientCollection.Object);

        _client = new Mock<IMongoClient>();
        _client.Setup(x => x.GetDatabase(It.IsAny<string?>(), null)).Returns(_context.Object);
    }

    #endregion

    #region Test Helper Methods

    void VerifyClientMapperBehavior()
    {
        foreach (ClientDocument clientDocument in _testClientDataset)
        { 
            ClientDto mapperResult = ClientMapper.MapClientDocument_ClientDto(clientDocument);
            mapperResult.Id.ShouldBe(clientDocument.Id);
            mapperResult.Address.ShouldBe(clientDocument.Address);
            mapperResult.MarkupRate.ShouldBe(clientDocument.MarkupRate);
            mapperResult.Name.ShouldBe(clientDocument.Name);
        }
    }

    #endregion

    #region Method Tests

    [UnitTestMethod]
    public async Task SuccessfulWhen_TwentyClients_InTestDataset()
    {
        // ARRANGE
        SetupTestDataset();
        SetupMocks();

        GetAllCleints_ToDto.Handler sut = new GetAllCleints_ToDto.Handler(_client.Object, _dbConfig);

        // ACT
        ClientDto? result = await sut.Handle(new GetAllCleints_ToDto.Query(), default);

        // ASSERT
        VerifyClientMapperBehavior();

        result.ShouldNotBeNull();
        result.Clients.Count.ShouldBe(_testClientDataset.Count);
    }

    [UnitTestMethod]
    public async Task SuccessfulWhen_ZeroClients_InTestDataset()
    {
        // ARRANGE
        _testClientDataset = new List<ClientDocument>();
        SetupMocks();

        GetAllCleints_ToDto.Handler sut = new GetAllCleints_ToDto.Handler(_client.Object, _dbConfig);

        // ACT
        ClientDto? result = await sut.Handle(new GetAllCleints_ToDto.Query(), default);

        // ASSERT
        VerifyClientMapperBehavior();

        result.ShouldNotBeNull();
        result.Clients.Count.ShouldBe(0);
        result.Clients.Count.ShouldBe(_testClientDataset.Count);
    }

    #endregion
}