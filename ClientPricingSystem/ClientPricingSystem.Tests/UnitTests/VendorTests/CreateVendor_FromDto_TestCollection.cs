using Bogus;
using ClientPricingSystem.Configuration;
using ClientPricingSystem.Core.Documents;
using ClientPricingSystem.Core.Dtos;
using ClientPricingSystem.Core.Methods.Vendor;
using ClientPricingSystem.Core.Validators.Vendor;
using ClientPricingSystem.Tests.Configuration;
using ClientPricingSystem.Tests.Fakers;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Moq;
using Shouldly;

namespace ClientPricingSystem.Tests.UnitTests.VendorTests;
[UnitTestCollection]
public class CreateVendor_FromDto_TestCollection
{
    // Test setup variables
    IOptions<DatabaseConfiguration> _dbConfig;
    Mock<IMongoCollection<VendorDocument>> _vendorCollection;
    Mock<IMongoDatabase> _context;
    Mock<IMongoClient> _client;

    // Test state variables
    VendorDocument _DtoToDocument_MapperResult;

    #region Test Configuration Methods

    /* Setup Methods*/
    public void Setup()
    {
        SetupDbConfiguration();
        SetupMockVendorCollection();
        SetupMockContextAndClient();
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

    void SetupMockVendorCollection()
    {
        _vendorCollection = new Mock<IMongoCollection<VendorDocument>>();
        // Setup callback to record result of mapping the VendorDto object to the VendorDocument object
        _vendorCollection.Setup(x =>
            x.InsertOneAsync(
                It.IsAny<VendorDocument>(),
                It.IsAny<InsertOneOptions>(),
                It.IsAny<CancellationToken>()))
            .Callback((VendorDocument vendor, InsertOneOptions options, CancellationToken cancellationToken) => Record_DtoToDocument_MapperResult(vendor));
    }

    void SetupMockContextAndClient()
    {
        _context = new Mock<IMongoDatabase>();
        _context.Setup(x => x.GetCollection<VendorDocument>(It.IsAny<string>(), null)).Returns(_vendorCollection.Object);

        _client = new Mock<IMongoClient>();
        _client.Setup(x => x.GetDatabase(It.IsAny<string?>(), null)).Returns(_context.Object);
    }

    /* Callback Methods */
    void Record_DtoToDocument_MapperResult(VendorDocument vendor) 
    {
        _DtoToDocument_MapperResult = vendor;
    }

    #endregion

    #region Test Helper Methods

    bool ValidateTestCommand(CreateVendor_FromDto.Command command)
    {
        CreateVendor_FromDto_CommandValidator validator = new CreateVendor_FromDto_CommandValidator();
        FluentValidation.Results.ValidationResult validationResult = validator.Validate(command);
        return validationResult.IsValid;
    }

    #endregion

    #region Method Tests

    [UnitTestMethod]
    public void SuccessfulWhen_AllDtoFieldsPopulated()
    {
        // ARRANGE
        VendorDocument newVendor = VendorFaker.GetVendorFaker().Generate();
        VendorDto newVendorDto = new VendorDto
        { 
            Name = newVendor.Name,
            Domains = newVendor.Domains,
            Notes = newVendor.Notes
        };

        CreateVendor_FromDto.Handler sut = new CreateVendor_FromDto.Handler(_client.Object, _dbConfig);

        CreateVendor_FromDto.Command command = new CreateVendor_FromDto.Command
        {
            VendorDto = newVendorDto
        };

        //Validate test command
        if (!ValidateTestCommand(command))
            throw new ValidationException("Test command is not valid.");

        // ACT
        var result = sut.Handle(new CreateVendor_FromDto.Command{ VendorDto = newVendorDto }, default);

        // ASSERT
        // Verify the InsertOneAsync function was called exactly once
        _vendorCollection.Verify(x =>
            x.InsertOneAsync(
                It.IsAny<VendorDocument>(),
                It.IsAny<InsertOneOptions>(),
                It.IsAny<CancellationToken>()),
            Times.Once());

        // Verify mapper appropriately mapped the VendorDto object to the VendorDocument object
        _DtoToDocument_MapperResult.Name.ShouldBe(newVendorDto.Name);
        _DtoToDocument_MapperResult.Domains.ShouldBe(newVendorDto.Domains);
        _DtoToDocument_MapperResult.Notes.ShouldBe(newVendorDto.Notes);
    }

    [UnitTestMethod]
    public void SuccessfulWhen_RequiredDtoFieldsPopulated()
    {
        // ARRANGE
        VendorDocument newVendor = VendorFaker.GetVendorFaker().Generate();
        VendorDto newVendorDto = new VendorDto
        {
            Name = newVendor.Name
        };

        CreateVendor_FromDto.Handler sut = new CreateVendor_FromDto.Handler(_client.Object, _dbConfig);

        CreateVendor_FromDto.Command command = new CreateVendor_FromDto.Command
        {
            VendorDto = newVendorDto
        };

        //Validate test command
        if (!ValidateTestCommand(command))
            throw new ValidationException("Test command is not valid.");

        // ACT
        var result = sut.Handle(new CreateVendor_FromDto.Command { VendorDto = newVendorDto }, default);

        // ASSERT
        // Verify the InsertOneAsync function was called exactly once
        _vendorCollection.Verify(x =>
            x.InsertOneAsync(
                It.IsAny<VendorDocument>(),
                It.IsAny<InsertOneOptions>(),
                It.IsAny<CancellationToken>()),
            Times.Once());

        // Verify mapper appropriately mapped the VendorDto object to the VendorDocument object
        _DtoToDocument_MapperResult.Name.ShouldBe(newVendorDto.Name);
        _DtoToDocument_MapperResult.Domains.ShouldBeNull();
        _DtoToDocument_MapperResult.Notes.ShouldBeNull();
    }

    [UnitTestMethod]
    public void SuccessfulWhen_DomainsRaw_IsNotNullOrEmpty()
    {
        // ARRANGE
        VendorDocument newVendor = VendorFaker.GetVendorFaker().Generate();
        VendorDto newVendorDto = new VendorDto
        {
            Name = newVendor.Name,
            DomainsRaw = string.Join(", ", newVendor.Domains)
        };

        CreateVendor_FromDto.Handler sut = new CreateVendor_FromDto.Handler(_client.Object, _dbConfig);

        CreateVendor_FromDto.Command command = new CreateVendor_FromDto.Command
        {
            VendorDto = newVendorDto
        };

        //Validate test command
        if (!ValidateTestCommand(command))
            throw new ValidationException("Test command is not valid.");

        // ACT
        var result = sut.Handle(new CreateVendor_FromDto.Command { VendorDto = newVendorDto }, default);

        // ASSERT
        // Verify the InsertOneAsync function was called exactly once
        _vendorCollection.Verify(x =>
            x.InsertOneAsync(
                It.IsAny<VendorDocument>(),
                It.IsAny<InsertOneOptions>(),
                It.IsAny<CancellationToken>()),
            Times.Once());

        // Verify mapper appropriately mapped the VendorDto object to the VendorDocument object
        _DtoToDocument_MapperResult.Name.ShouldBe(newVendorDto.Name);
        _DtoToDocument_MapperResult.Domains.ShouldBe(newVendor.Domains);
        _DtoToDocument_MapperResult.Notes.ShouldBeNull();
    }

    #endregion
}