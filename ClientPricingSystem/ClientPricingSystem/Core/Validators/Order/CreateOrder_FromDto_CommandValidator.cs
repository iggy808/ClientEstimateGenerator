using ClientPricingSystem.Core.MediatRMethods.Order;
using FluentValidation;

namespace ClientPricingSystem.Core.Validators.Order;
public class CreateOrder_FromDto_CommandValidator : AbstractValidator<CreateOrder_FromDto.Command>
{
    public CreateOrder_FromDto_CommandValidator()
    {
        RuleFor(x => x.OrderDto)
            .NotNull()
            .DependentRules(() =>
            {
                RuleFor(x => x.OrderDto.ClientId)
                    .NotNull().NotEqual(Guid.Empty);

                RuleFor(x => x.OrderDto.ArtistFee)
                    .NotNull().GreaterThan(0.00m);

                RuleFor(x => x.OrderDto.Items)
                    .NotNull().NotEmpty()
                    .When(x => string.IsNullOrEmpty(x.OrderDto.ItemsJson));

                RuleFor(x => x.OrderDto.ItemsJson)
                    .NotNull().NotEmpty()
                    .NotEqual("[{\"VendorId\": \"\", \"Size\": \"\", \"ArticleQuantity\": \"\", \"Total\": \"\"}]")
                    .When(x => x.OrderDto.Items == null || !x.OrderDto.Items.Any());
            });
    }
}