using ClientPricingSystem.Core.MediatRMethods.Order.OrderItem;
using FluentValidation;

namespace ClientPricingSystem.Core.Validators.Order.OrderItem;
public class CreateOrderItems_CommandValidator : AbstractValidator<CreateOrderItems.Command>
{
    public CreateOrderItems_CommandValidator()
    {
        RuleFor(x => x.Items)
            .NotNull().NotEmpty()
            .DependentRules(() =>
            {
                RuleFor(x => x.Items.Sum(i => i.ArticleQuantity * i.UnitPrice))
                    .GreaterThan(0.00m);
            });

        RuleFor(x => x.OrderId)
            .NotNull().NotEqual(Guid.Empty);
    }
}