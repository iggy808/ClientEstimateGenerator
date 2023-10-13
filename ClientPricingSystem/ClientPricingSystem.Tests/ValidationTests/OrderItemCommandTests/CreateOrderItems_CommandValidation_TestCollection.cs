using ClientPricingSystem.Core.Documents;
using ClientPricingSystem.Core.Dtos;
using ClientPricingSystem.Core.Methods.Order.OrderItem;
using ClientPricingSystem.Core.Validators.Order.OrderItem;
using ClientPricingSystem.Tests.Configuration;
using ClientPricingSystem.Tests.Fakers;
using FluentValidation.Results;
using Shouldly;

namespace ClientPricingSystem.Tests.ValidationTests.OrderItemCommandTests;
[ValidationTestCollection]
public class CreateOrderItems_CommandValidation_TestCollection
{
    CreateOrderItems_CommandValidator _validator;

    #region Test Configuration Methods

    /* Setup Methods */
    void SetupValidator()
    {
        _validator = new CreateOrderItems_CommandValidator();
    }

    #endregion

    #region Validator Tests

    /* Positive Tests */
    [ValidationTestMethod]
    public void Validation_SuccessfulWhen_AllCommandFields_AreValid()
    {
        // ARRANGE
        SetupValidator();

        OrderDocument newOrder = OrderFaker.GetOrderFaker().Generate();
        CreateOrderItems.Command command = new CreateOrderItems.Command
        {
            Order = newOrder
        };

        // ACT
        ValidationResult validationResult = _validator.Validate(command);

        // ASSERT
        validationResult.IsValid.ShouldBeTrue();
        validationResult.Errors.Count.ShouldBe(0);
    }

    /* Negative Tests */
    [ValidationTestMethod]
    public void Validation_SuccessfulWhen_ItemsSum_IsInvalid()
    {
        // ARRANGE
        SetupValidator();

        OrderDocument newOrder = OrderFaker.GetOrderFaker().Generate();
        newOrder.Items.ForEach(i =>
        {
            i.UnitPrice = 0.00m;
            i.ArticleQuantity = 0;
        });

        CreateOrderItems.Command command = new CreateOrderItems.Command
        {
            Order = newOrder
        };

        // ACT
        ValidationResult validationResult = _validator.Validate(command);

        // ASSERT
        validationResult.IsValid.ShouldBeFalse();
        validationResult.Errors.Count.ShouldBe(1);
    }

    [ValidationTestMethod]
    public void Validation_SuccessfulWhen_OrderId_IsEmpty()
    {
        // ARRANGE
        SetupValidator();

        CreateOrderItems.Command command = new CreateOrderItems.Command
        {
            Order = new OrderDocument
            { 
                Id = Guid.Empty,
                Items = OrderItemFaker.GetOrderItemFaker().Generate(2)
            }
        };

        // ACT
        ValidationResult validationResult = _validator.Validate(command);

        // ASSERT
        validationResult.IsValid.ShouldBeFalse();
        validationResult.Errors.Count.ShouldBe(1);
    }

    [ValidationTestMethod]
    public void Validation_SuccessfulWhen_Items_IsNull()
    {
        // ARRANGE
        SetupValidator();

        CreateOrderItems.Command command = new CreateOrderItems.Command
        {
            Order = new OrderDocument
            {
                Id = Guid.NewGuid(),
                Items = null
            }
        };

        // ACT
        ValidationResult validationResult = _validator.Validate(command);

        // ASSERT
        validationResult.IsValid.ShouldBeFalse();
        validationResult.Errors.Count.ShouldBe(2);
    }

    [ValidationTestMethod]
    public void Validation_SuccessfulWhen_Items_IsEmpty()
    {
        // ARRANGE
        SetupValidator();

        CreateOrderItems.Command command = new CreateOrderItems.Command
        {
            Order = new OrderDocument
            {
                Id = Guid.NewGuid(),
                Items = new List<OrderItemDocument>()
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