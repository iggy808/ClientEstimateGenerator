using ClientPricingSystem.Core.Documents;
using ClientPricingSystem.Core.Dtos;
using ClientPricingSystem.Core.Methods.Order;
using ClientPricingSystem.Core.Validators.Order;
using ClientPricingSystem.Tests.Configuration;
using ClientPricingSystem.Tests.Fakers;
using FluentValidation.Results;
using Newtonsoft.Json;
using Shouldly;

namespace ClientPricingSystem.Tests.ValidationTests.OrderCommandTests;
[ValidationTestCollection]
public class CreateOrder_FromDto_CommandValidation_TestCollection
{
    const decimal TAX = 10.00m;

    CreateOrder_FromDto_CommandValidator _validator;

    #region Test Configuration Methods

    /* Setup Methods */
    void SetupValidator()
    {
        _validator = new CreateOrder_FromDto_CommandValidator();
    }

    #endregion

    #region Test Helper Methods

    // Note: Method returns array of 2 decimal values.
    //         - The first element in the array is the subtotal
    //         - The second element in the array is the total
    decimal[] Get_Subtotal_And_Total(List<OrderItemDocument> items)
    {
        decimal newOrderSubtotal = 0.00m;
        foreach (OrderItemDocument item in items)
        {
            newOrderSubtotal += item.ArticleQuantity * item.UnitPrice;
        }
        decimal newOrderTotal = newOrderSubtotal + TAX;

        return new decimal[] { newOrderSubtotal, newOrderTotal };
    }

    #endregion

    #region Validator Tests

    /* Positive Tests */
    [ValidationTestMethod]
    public void Validation_SuccessfulWhen_AllDtoFields_AreValid_Case1()
    {
        // ARRANGE
        SetupValidator();

        OrderDocument newOrder = OrderFaker.GetOrderFaker().Generate();
        decimal[] totals = Get_Subtotal_And_Total(newOrder.Items); 

        CreateOrder_FromDto.Command command = new CreateOrder_FromDto.Command
        {
            OrderDto = new OrderDto
            {
                Client = newOrder.Client,
                ClientId = newOrder.Client.Id,
                ArtistFee = newOrder.ArtistFee,
                Items = newOrder.Items,
                SubTotal = totals[0],
                Total = totals[1]
            }
        };

        // ACT
        ValidationResult validationResult = _validator.Validate(command);

        // ASSERT
        validationResult.IsValid.ShouldBeTrue();
        validationResult.Errors.Count.ShouldBe(0);
    }

    [ValidationTestMethod]
    public void Validation_SuccessfulWhen_AllDtoFields_AreValid_Case2()
    {
        // ARRANGE
        SetupValidator();

        OrderDocument newOrder = OrderFaker.GetOrderFaker().Generate();
        decimal[] totals = Get_Subtotal_And_Total(newOrder.Items);

        CreateOrder_FromDto.Command command = new CreateOrder_FromDto.Command
        {
            OrderDto = new OrderDto
            {
                Client = newOrder.Client,
                ClientId = newOrder.Client.Id,
                ArtistFee = newOrder.ArtistFee,
                ItemsJson = JsonConvert.SerializeObject(newOrder.Items),
                SubTotal = totals[0],
                Total = totals[1]
            }
        };

        // ACT
        ValidationResult validationResult = _validator.Validate(command);

        // ASSERT
        validationResult.IsValid.ShouldBeTrue();
        validationResult.Errors.Count.ShouldBe(0);
    }

    [ValidationTestMethod]
    public void Validation_SuccessfulWhen_RequiredDtoFields_AreValid_Case1()
    {
        // ARRANGE
        SetupValidator();

        OrderDocument newOrder = OrderFaker.GetOrderFaker().Generate();
        decimal[] totals = Get_Subtotal_And_Total(newOrder.Items);

        CreateOrder_FromDto.Command command = new CreateOrder_FromDto.Command
        {
            OrderDto = new OrderDto
            {
                ClientId = newOrder.Client.Id,
                ArtistFee = newOrder.ArtistFee,
                ItemsJson = JsonConvert.SerializeObject(newOrder.Items)
            }
        };

        // ACT
        ValidationResult validationResult = _validator.Validate(command);

        // ASSERT
        validationResult.IsValid.ShouldBeTrue();
        validationResult.Errors.Count.ShouldBe(0);
    }

    [ValidationTestMethod]
    public void Validation_SuccessfulWhen_RequiredDtoFields_AreValid_Case2()
    {
        // ARRANGE
        SetupValidator();

        OrderDocument newOrder = OrderFaker.GetOrderFaker().Generate();

        CreateOrder_FromDto.Command command = new CreateOrder_FromDto.Command
        {
            OrderDto = new OrderDto
            {
                ClientId = newOrder.Client.Id,
                ArtistFee = newOrder.ArtistFee,
                Items = newOrder.Items
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
    public void Validation_SuccessfulWhen_OrderDto_IsNull()
    {
        // ARRANGE
        SetupValidator();

        CreateOrder_FromDto.Command command = new CreateOrder_FromDto.Command
        {
            OrderDto = null
        };

        // ACT
        ValidationResult validationResult = _validator.Validate(command);

        // ASSERT
        validationResult.IsValid.ShouldBeFalse();
        validationResult.Errors.Count.ShouldBe(1);
    }

    [ValidationTestMethod]
    public void Validation_SuccessfulWhen_ClientId_IsNull()
    {
        // ARRANGE
        SetupValidator();

        OrderDocument newOrder = OrderFaker.GetOrderFaker().Generate();

        CreateOrder_FromDto.Command command = new CreateOrder_FromDto.Command
        {
            OrderDto = new OrderDto
            {
                ClientId = Guid.Empty,
                ArtistFee = newOrder.ArtistFee,
                Items = newOrder.Items
            }
        };

        // ACT
        ValidationResult validationResult = _validator.Validate(command);

        // ASSERT
        validationResult.IsValid.ShouldBeFalse();
        validationResult.Errors.Count.ShouldBe(1);
    }

    [ValidationTestMethod]
    public void Validation_SuccessfulWhen_ArtistFee_IsZero()
    {
        // ARRANGE
        SetupValidator();

        OrderDocument newOrder = OrderFaker.GetOrderFaker().Generate();

        CreateOrder_FromDto.Command command = new CreateOrder_FromDto.Command
        {
            OrderDto = new OrderDto
            {
                ClientId = newOrder.Client.Id,
                ArtistFee = 0.00m,
                Items = newOrder.Items
            }
        };

        // ACT
        ValidationResult validationResult = _validator.Validate(command);

        // ASSERT
        validationResult.IsValid.ShouldBeFalse();
        validationResult.Errors.Count.ShouldBe(1);
    }

    [ValidationTestMethod]
    public void Validation_SuccessfulWhen_ArtistFee_IsNegative()
    {
        // ARRANGE
        SetupValidator();

        OrderDocument newOrder = OrderFaker.GetOrderFaker().Generate();

        CreateOrder_FromDto.Command command = new CreateOrder_FromDto.Command
        {
            OrderDto = new OrderDto
            {
                ClientId = newOrder.Client.Id,
                ArtistFee = -15.75m,
                Items = newOrder.Items
            }
        };

        // ACT
        ValidationResult validationResult = _validator.Validate(command);

        // ASSERT
        validationResult.IsValid.ShouldBeFalse();
        validationResult.Errors.Count.ShouldBe(1);
    }

    [ValidationTestMethod]
    public void Validation_SuccessfulWhen_ItemsIsEmpty_And_ItemsJsonIsNull()
    {
        // ARRANGE
        SetupValidator();

        OrderDocument newOrder = OrderFaker.GetOrderFaker().Generate();

        CreateOrder_FromDto.Command command = new CreateOrder_FromDto.Command
        {
            OrderDto = new OrderDto
            {
                ClientId = newOrder.Client.Id,
                ArtistFee = newOrder.ArtistFee,
                Items = new List<OrderItemDocument>(),
                ItemsJson = null
            }
        };

        // ACT
        ValidationResult validationResult = _validator.Validate(command);

        // ASSERT
        validationResult.IsValid.ShouldBeFalse();
        validationResult.Errors.Count.ShouldBe(3);
    }

    [ValidationTestMethod]
    public void Validation_SuccessfulWhen_ItemsIsNull_And_ItemsJsonIsInvalid()
    {
        // ARRANGE
        SetupValidator();

        OrderDocument newOrder = OrderFaker.GetOrderFaker().Generate();

        CreateOrder_FromDto.Command command = new CreateOrder_FromDto.Command
        {
            OrderDto = new OrderDto
            {
                ClientId = newOrder.Client.Id,
                ArtistFee = newOrder.ArtistFee,
                Items = null,
                ItemsJson = "[{\"VendorId\": \"\", \"Size\": \"\", \"ArticleQuantity\": \"\", \"Total\": \"\"}]"
            }
        };

        // ACT
        ValidationResult validationResult = _validator.Validate(command);

        // ASSERT
        validationResult.IsValid.ShouldBeFalse();
        validationResult.Errors.Count.ShouldBe(1);
    }

    [ValidationTestMethod]
    public void Validation_SuccessfulWhen_ItemsAndItemsJson_AreNull()
    {
        // ARRANGE
        SetupValidator();

        OrderDocument newOrder = OrderFaker.GetOrderFaker().Generate();

        CreateOrder_FromDto.Command command = new CreateOrder_FromDto.Command
        {
            OrderDto = new OrderDto
            {
                ClientId = newOrder.Client.Id,
                ArtistFee = newOrder.ArtistFee,
                Items = null,
                ItemsJson = null
            }
        };

        // ACT
        ValidationResult validationResult = _validator.Validate(command);

        // ASSERT
        validationResult.IsValid.ShouldBeFalse();
        validationResult.Errors.Count.ShouldBe(4);
    }

    #endregion
}