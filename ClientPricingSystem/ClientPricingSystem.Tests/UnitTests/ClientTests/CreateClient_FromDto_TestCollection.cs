using Bogus;
using ClientPricingSystem.Configuration;
using ClientPricingSystem.Core.Documents;
using ClientPricingSystem.Core.Dtos;
using ClientPricingSystem.Core.MediatRMethods.Client;
using ClientPricingSystem.Core.Validators.Client;
using ClientPricingSystem.Tests.Configuration;
using ClientPricingSystem.Tests.Fakers;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Moq;
using Shouldly;

namespace ClientPricingSystem.Tests.UnitTests.ClientTests;
[UnitTestCollection]
public class CreateClient_FromDto_TestCollection
{
    // Test setup variables
    IOptions<DatabaseConfiguration> _dbConfig;
    Mock<IMongoCollection<ClientDocument>> _clientCollection;
    Mock<IMongoDatabase> _context;
    Mock<IMongoClient> _client;

    // Test state variables
    ClientDocument _DtoToDocument_MapperResult;

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

        _clientCollection = new Mock<IMongoCollection<ClientDocument>>();
        // Setup callback to record result of mapping the ClientDto object to the ClientDocument object
        _clientCollection.Setup(x =>
            x.InsertOneAsync(
                It.IsAny<ClientDocument>(),
                It.IsAny<InsertOneOptions>(),
                It.IsAny<CancellationToken>()))
            .Callback((ClientDocument client, InsertOneOptions options, CancellationToken cancellationToken) => Record_DtoToDocument_MapperResult(client));

        _context = new Mock<IMongoDatabase>();
        _context.Setup(x => x.GetCollection<ClientDocument>(It.IsAny<string>(), null)).Returns(_clientCollection.Object);

        _client = new Mock<IMongoClient>();
        _client.Setup(x => x.GetDatabase(It.IsAny<string?>(), null)).Returns(_context.Object);
    }

    /* Callback Methods */
    void Record_DtoToDocument_MapperResult(ClientDocument client) 
    {
        _DtoToDocument_MapperResult = client;
    }

    #endregion

    #region Test Helper Methods

    bool ValidateTestCommand(CreateClient_FromDto.Command command)
    {
        CreateClient_FromDto_CommandValidator validator = new CreateClient_FromDto_CommandValidator();
        FluentValidation.Results.ValidationResult validationResult = validator.Validate(command);
        return validationResult.IsValid;
    }

    #endregion

    #region Method Tests

    [UnitTestMethod]
    public void SuccessfulWhen_AllDtoFieldsPopulated()
    {
        // ARRANGE
        SetupMocks();

        ClientDocument newClient = ClientFaker.GetClientFaker().Generate();
        ClientDto newClientDto = new ClientDto
        { 
            Name = newClient.Name,
            Address = newClient.Address,
            MarkupRate = newClient.MarkupRate
        };

        CreateClient_FromDto.Handler sut = new CreateClient_FromDto.Handler(_client.Object, _dbConfig);

        CreateClient_FromDto.Command command = new CreateClient_FromDto.Command
        {
            ClientDto = newClientDto
        };

        //Validate test command
        if (!ValidateTestCommand(command))
            throw new ValidationException("Test command is not valid.");

        // ACT
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

    [UnitTestMethod]
    public void SuccessfulWhen_RequiredDtoFieldsPopulated()
    {
        // ARRANGE
        SetupMocks();

        ClientDocument newClient = ClientFaker.GetClientFaker().Generate();
        ClientDto newClientDto = new ClientDto
        {
            Name = newClient.Name,
            MarkupRate = newClient.MarkupRate
        };

        CreateClient_FromDto.Handler sut = new CreateClient_FromDto.Handler(_client.Object, _dbConfig);

        CreateClient_FromDto.Command command = new CreateClient_FromDto.Command
        {
            ClientDto = newClientDto
        };

        //Validate test command
        if (!ValidateTestCommand(command))
            throw new ValidationException("Test command is not valid.");

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
        _DtoToDocument_MapperResult.Name.ShouldBe(newClientDto.Name);
        _DtoToDocument_MapperResult.MarkupRate.ShouldBe(newClientDto.MarkupRate);
        _DtoToDocument_MapperResult.Address.ShouldBeNull();
    }

    #endregion
}