using ClientPricingSystem.Core.Documents;
using ClientPricingSystem.Core.Dtos;
using ClientPricingSystem.Core.MediatRMethods.Client;
using ClientPricingSystem.Core.Validators.Client;
using ClientPricingSystem.Tests.Configuration;
using ClientPricingSystem.Tests.Fakers;
using FluentValidation.Results;
using Shouldly;

namespace ClientPricingSystem.Tests.ValidationTests.ClientCommandTests;
[ValidationTestCollection]
public class CreateClient_FromDto_CommandValidation_TestCollection
{
    CreateClient_FromDto_CommandValidator _validator;

    #region Test Configuration Methods

    /* Setup Methods */
    void SetupValidator()
    {
        _validator = new CreateClient_FromDto_CommandValidator();
    }

    #endregion

    #region Command Tests

    // Positive Tests
    [ValidationTestMethod]
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

    [ValidationTestMethod]
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
    [ValidationTestMethod]
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

    [ValidationTestMethod]
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

    [ValidationTestMethod]
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
}