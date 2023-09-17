using ClientPricingSystem.Core.MediatRMethods.Order.OrderItem;
using FluentValidation;

namespace ClientPricingSystem.Core.Validators.Order.OrderItem;
public class CreateOrderItems_CommandValidator : AbstractValidator<CreateOrderItems.Command>
{
    public CreateOrderItems_CommandValidator()
    {
        RuleFor(x => x.Order)
            .NotNull()
            .DependentRules(() =>
            {
                RuleFor(x => x.Order.Id)
                    .NotNull().NotEqual(Guid.Empty);

                RuleFor(x => x.Order.Items)
                .NotNull().NotEmpty()
                .DependentRules(() =>
                {
                    RuleFor(x => x.Order.Items.Sum(i => i.ArticleQuantity * i.UnitPrice))
                        .GreaterThan(0.00m);
                });
            });
    }
}