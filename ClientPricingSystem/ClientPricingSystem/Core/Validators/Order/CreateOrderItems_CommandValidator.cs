using ClientPricingSystem.Core.MediatRMethods.Order;
using FluentValidation;

namespace ClientPricingSystem.Core.Validators.Order;
public class CreateOrderItems_CommandValidator : AbstractValidator<CreateOrderItems.Command>
{
    public CreateOrderItems_CommandValidator()
    {
        RuleFor(x => x.Items)
            .NotNull().NotEmpty();

        RuleFor(x => x.OrderId)
            .NotNull().NotEqual(Guid.Empty);
    }
}