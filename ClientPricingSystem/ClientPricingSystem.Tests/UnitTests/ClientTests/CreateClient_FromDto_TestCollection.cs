using ClientPricingSystem.Configuration;
using ClientPricingSystem.Core.Documents;
using ClientPricingSystem.Core.Dtos;
using ClientPricingSystem.Core.MediatRMethods.Client;
using ClientPricingSystem.Tests.Configuration;
using ClientPricingSystem.Tests.Fakers;
using MediatR;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Moq;
using Shouldly;
using System.Xml.Linq;

namespace ClientPricingSystem.Tests.UnitTests.ClientTests;
[UnitTestCollection]
public class CreateClient_FromDto_TestCollection
{
    // Test constants
    const int TestClientRecordCount = 20;

    // Test setup variables
    IOptions<DatabaseConfiguration> _dbConfig;
    Mock<IMongoCollection<ClientDocument>> _clientCollection;
    Mock<IMongoDatabase> _context;
    Mock<IMongoClient> _client;

    // Test state variables
    List<ClientDocument> _TestClients;
    ClientDocument _DtoToDocument_MapperResult;

    #region Test Preparation Methods

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

        _clientCollection = new Mock<IMongoCollection<ClientDocument>>();
        _clientCollection.Setup(x =>
            x.InsertOneAsync(
                It.IsAny<ClientDocument>(),
                It.IsAny<InsertOneOptions>(),
                It.IsAny<CancellationToken>()))
            .Callback((ClientDocument client, InsertOneOptions options, CancellationToken cancellationToken) => AddClientToTestDataset(client));

        _context = new Mock<IMongoDatabase>();
        _context.Setup(x => x.GetCollection<ClientDocument>(It.IsAny<string>(), null)).Returns(_clientCollection.Object);

        _client = new Mock<IMongoClient>();
        _client.Setup(x => x.GetDatabase(It.IsAny<string?>(), null)).Returns(_context.Object);

        if (_TestClients == null)
            _TestClients = new List<ClientDocument>();
    }

    void SetupTestData()
    {
        _TestClients = ClientFaker
            .GetClientFaker()
            .Generate(TestClientRecordCount);
    }

    /* Callback Methods */
    void AddClientToTestDataset(ClientDocument client)
    {
        _TestClients.Add(client);
    }

    void Record_DtoToDocument_MapperResult(ClientDocument client) 
    {
        _DtoToDocument_MapperResult = client;
    }

    #endregion

    [UnitTestMethod]
    public void SuccessfulWhen_AllNecessaryDtoFieldsPopulated()
    {
        // ARRANGE
        SetupMocks();

        // Setup callback to record result of mapping the ClientDto object to the ClientDocument object
        _clientCollection.Setup(x =>
            x.InsertOneAsync(
                It.IsAny<ClientDocument>(),
                It.IsAny<InsertOneOptions>(),
                It.IsAny<CancellationToken>()))
            .Callback((ClientDocument client, InsertOneOptions options, CancellationToken cancellationToken) => Record_DtoToDocument_MapperResult(client));

        ClientDocument newClient = ClientFaker.GetClientFaker().Generate();
        ClientDto newClientDto = new ClientDto
        { 
            Name = newClient.Name,
            Address = newClient.Address,
            MarkupRate = newClient.MarkupRate
        };

        // ACT
        CreateClient_FromDto.Handler sut = new CreateClient_FromDto.Handler(_client.Object, _dbConfig);
        var result = sut.Handle(new CreateClient_FromDto.Command{ ClientDto = newClientDto }, default);

        // ASSERT
        // Verify the InsertOneAsync function was called exactly once
        _clientCollection.Verify(x =>
            x.InsertOneAsync(
                It.IsAny<ClientDocument>(),
                It.IsAny<InsertOneOptions>(),
                It.IsAny<CancellationToken>()),
            Times.Once());

        // Verify mapper appropriately mapped the ClientDto object to the ClientDocument object
        _DtoToDocument_MapperResult.Name.ShouldBe(newClientDto.Name);
        _DtoToDocument_MapperResult.Address.ShouldBe(newClientDto.Address);
        _DtoToDocument_MapperResult.MarkupRate.ShouldBe(newClientDto.MarkupRate);
    }
}
