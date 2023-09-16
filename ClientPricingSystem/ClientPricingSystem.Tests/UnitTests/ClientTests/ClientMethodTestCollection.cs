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
public class ClientMethodTestCollection
{
    List<ClientDocument> _TestClients;
    ClientDocument _DtoToDocument_MapperResult;

    Mock<IMongoClient> _client;
    Mock<IMongoDatabase> _context;
    Mock<IMongoCollection<ClientDocument>> _clientCollection;
    Mock<IOptions<DatabaseConfiguration>> _dbConfig;

    const int TestClientRecordCount = 20;

    #region Test Preparation Methods

    /* Setup Methods*/
    void SetupTestData()
    {
        _TestClients = ClientFaker
            .GetClientFaker()
            .Generate(TestClientRecordCount);
    }

    void SetupMocks()
    {
        _dbConfig = new Mock<IOptions<DatabaseConfiguration>>();
        _dbConfig.Setup(x => x.Value.DatabaseName).Returns(TestDatabase.DatabaseName);
        _dbConfig.Setup(x => x.Value).Returns(_dbConfig.Object.Value);

        _clientCollection = new Mock<IMongoCollection<ClientDocument>>();
        _clientCollection.Setup(x =>
            x.InsertOneAsync(
                It.IsAny<ClientDocument>(),
                It.IsAny<InsertOneOptions>(),
                It.IsAny<CancellationToken>())
            .ConfigureAwait(false))
            .Callback((ClientDocument client) => AddClientToTestDataset(client));

        _context = new Mock<IMongoDatabase>();
        _context.Setup(x => x.GetCollection<ClientDocument>(It.IsAny<string>(), null)).Returns(_clientCollection.Object);

        _client = new Mock<IMongoClient>();
        _client.Setup(x => x.GetDatabase(It.IsAny<string?>(), null)).Returns(_context.Object);
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
    public void CreateClient_FromDto_Test()
    {
        // ARRANGE
        SetupTestData();
        SetupMocks();

        // Mock callback to record result of mapping ClientDto -> ClientDocument through mapper
        _clientCollection.Setup(x =>
            x.InsertOneAsync(
                It.IsAny<ClientDocument>(),
                It.IsAny<InsertOneOptions>(),
                It.IsAny<CancellationToken>())
            .ConfigureAwait(false))
            .Callback((ClientDocument client) => Record_DtoToDocument_MapperResult(client));

        ClientDocument newClient = ClientFaker.GetClientFaker().Generate();
        ClientDto newClientDto = new ClientDto
        { 
            Name = newClient.Name,
            Address = newClient.Address,
            MarkupRate = newClient.MarkupRate
        };

        // ACT
        CreateClient_FromDto.Handler sut = new CreateClient_FromDto.Handler(_client.Object, _dbConfig.Object);
        var result = sut.Handle(new CreateClient_FromDto.Command{ ClientDto = newClientDto }, default);

        // ASSERT
        // Verify mapper appropriately mapped ClientDto -> ClientDocument
        _DtoToDocument_MapperResult.Name.ShouldBe(newClientDto.Name);
        _DtoToDocument_MapperResult.Address.ShouldBe(newClientDto.Address);
        _DtoToDocument_MapperResult.MarkupRate.ShouldBe(newClientDto.MarkupRate);
    }
}
