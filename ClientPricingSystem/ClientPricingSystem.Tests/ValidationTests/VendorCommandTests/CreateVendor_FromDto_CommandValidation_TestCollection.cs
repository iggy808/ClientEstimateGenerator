using ClientPricingSystem.Core.Documents;
using ClientPricingSystem.Core.Dtos;
using ClientPricingSystem.Core.MediatRMethods.Vendor;
using ClientPricingSystem.Core.Validators.Vendor;
using ClientPricingSystem.Tests.Configuration;
using ClientPricingSystem.Tests.Fakers;
using FluentValidation.Results;
using Shouldly;

namespace ClientPricingSystem.Tests.ValidationTests.VendorCommandTests;
[ValidationTestCollection]
public class CreateVendor_FromDto_CommandValidation_TestCollection
{
    CreateVendor_FromDto_CommandValidator _validator;

    #region Test Configuration Methods

    /* Setup Methods */
    void SetupValidator()
    {
        _validator = new CreateVendor_FromDto_CommandValidator();
    }

    #endregion

    #region Validator Tests

    /* Positive Tests */
    [ValidationTestMethod]
    public void Validation_SuccessfulWhen_AllDtoFields_AreValid()
    {
        // ARRANGE
        SetupValidator();

        VendorDocument newVendor = VendorFaker.GetVendorFaker().Generate();
        CreateVendor_FromDto.Command command = new CreateVendor_FromDto.Command
        {
            VendorDto = new VendorDto
            {
                Name = newVendor.Name,
                Domains = newVendor.Domains,
                Notes = newVendor.Notes
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

        VendorDocument newVendor = VendorFaker.GetVendorFaker().Generate();
        CreateVendor_FromDto.Command command = new CreateVendor_FromDto.Command
        {
            VendorDto = new VendorDto
            {
                Name = newVendor.Name
            }
        };

        // ACT
        ValidationResult validationResult = _validator.Validate(command);

        // ASSERT
        validationResult.IsValid.ShouldBeTrue();
        validationResult.Errors.Count.ShouldBe(0);
    }

    /* Negative Tests */
    [ValidationTestMethod]
    public void Validation_SuccessfulWhen_VendorDto_IsNull()
    {
        // ARRANGE
        SetupValidator();

        CreateVendor_FromDto.Command command = new CreateVendor_FromDto.Command
        {
            VendorDto = null
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

        CreateVendor_FromDto.Command command = new CreateVendor_FromDto.Command
        {
            VendorDto = new VendorDto
            {
                Name = null
            }
        };

        // ACT
        ValidationResult validationResult = _validator.Validate(command);

        // ASSERT
        validationResult.IsValid.ShouldBeFalse();
        validationResult.Errors.Count.ShouldBe(2);
    }

    [ValidationTestMethod]
    public void Validation_SuccessfulWhen_RequiredDtoFields_AreInvalid_Case2()
    {
        // ARRANGE
        SetupValidator();

        CreateVendor_FromDto.Command command = new CreateVendor_FromDto.Command
        {
            VendorDto = new VendorDto
            {
                Name = "",
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