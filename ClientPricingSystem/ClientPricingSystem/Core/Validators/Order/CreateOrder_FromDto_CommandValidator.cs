using ClientPricingSystem.Core.MediatRMethods.Order;
using FluentValidation;

namespace ClientPricingSystem.Core.Validators.Order;
public class CreateOrder_FromDto_CommandValidator : AbstractValidator<CreateOrder_FromDto.Command>
{
    public CreateOrder_FromDto_CommandValidator()
    {
        RuleFor(x => x.OrderDto)
            .NotNull();

        RuleFor(x => x.OrderDto.ClientId)
            .NotNull().NotEqual(Guid.Empty);

        RuleFor(x => x.OrderDto.ArtistFee)
            .NotNull().GreaterThanOrEqualTo(0.01m);

        RuleFor(x => x.OrderDto.ItemsJson)
            .NotNull().NotEmpty().NotEqual("[{\"VendorId\": \"\", \"Size\": \"\", \"ArticleQuantity\": \"\", \"Total\": \"\"}]");
    }
}