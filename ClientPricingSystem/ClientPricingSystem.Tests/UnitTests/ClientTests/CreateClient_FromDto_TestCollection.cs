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
using Moq;
using Shouldly;

namespace ClientPricingSystem.Tests.UnitTests.ClientTests;
[UnitTestCollection]
public class CreateClient_FromDto_TestCollection
{
    // Test setup variables
    CreateClient_FromDto_CommandValidator _validator;
    IOptions<DatabaseConfiguration> _dbConfig;
    Mock<IMongoCollection<ClientDocument>> _clientCollection;
    Mock<IMongoDatabase> _context;
    Mock<IMongoClient> _client;

    // Test state variables
    ClientDocument _DtoToDocument_MapperResult;

    #region Test Configuration Methods

    /* Setup Methods*/
    void SetupValidator()
    {
        _validator = new CreateClient_FromDto_CommandValidator();
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

    #region Validation Tests

    // Positive Tests
    [UnitTestMethod]
    public void Validation_SuccessfulWhen_AllDtoFields_AreValid()
    {
        // ARRANGE
        SetupValidator();

        ClientDocument newClient = ClientFaker.GetClientFaker().Generate();
        CreateClient_FromDto.Command command = new CreateClient_FromDto.Command
        {
            ClientDto = new ClientDto
            {
                Name = newClient.Name,
                Address = newClient.Address,
                MarkupRate = newClient.MarkupRate
            }
        };

        // ACT
        ValidationResult validationResult = _validator.Validate(command);

        // ASSERT
        validationResult.IsValid.ShouldBeTrue();
        validationResult.Errors.Count.ShouldBe(0);
    }

    [UnitTestMethod]
    public void Validation_SuccessfulWhen_RequiredDtoFields_AreValid()
    {
        // ARRANGE
        SetupValidator();

        ClientDocument newClient = ClientFaker.GetClientFaker().Generate();
        CreateClient_FromDto.Command command = new CreateClient_FromDto.Command
        {
            ClientDto = new ClientDto
            {
                Name = newClient.Name,
                MarkupRate = newClient.MarkupRate
            }
        };

        // ACT
        ValidationResult validationResult = _validator.Validate(command);

        // ASSERT
        validationResult.IsValid.ShouldBeTrue();
        validationResult.Errors.Count.ShouldBe(0);
    }

    // Negative Tests
    [UnitTestMethod]
    public void Validation_SuccessfulWhen_ClientDto_IsNull()
    {
        // ARRANGE
        SetupValidator();

        CreateClient_FromDto.Command command = new CreateClient_FromDto.Command
        {
            ClientDto = null
        };

        // ACT
        ValidationResult validationResult = _validator.Validate(command);

        // ASSERT
        validationResult.IsValid.ShouldBeFalse();
        validationResult.Errors.Count.ShouldBe(1);
    }

    [UnitTestMethod]
    public void Validation_SuccessfulWhen_RequiredDtoFields_AreInvalid_Case1()
    {
        // ARRANGE
        SetupValidator();

        CreateClient_FromDto.Command command = new CreateClient_FromDto.Command
        {
            ClientDto = new ClientDto
            {
                Name = null,
                MarkupRate = 0.00m
            }
        };

        // ACT
        ValidationResult validationResult = _validator.Validate(command);

        // ASSERT
        validationResult.IsValid.ShouldBeFalse();
        validationResult.Errors.Count.ShouldBe(3);
    }

    [UnitTestMethod]
    public void Validation_SuccessfulWhen_RequiredDtoFields_AreInvalid_Case2()
    {
        // ARRANGE
        SetupValidator();

        CreateClient_FromDto.Command command = new CreateClient_FromDto.Command
        {
            ClientDto = new ClientDto
            {
                Name = "",
                MarkupRate = -15.75m
            }
        };

        // ACT
        ValidationResult validationResult = _validator.Validate(command);

        // ASSERT
        validationResult.IsValid.ShouldBeFalse();
        validationResult.Errors.Count.ShouldBe(1);
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