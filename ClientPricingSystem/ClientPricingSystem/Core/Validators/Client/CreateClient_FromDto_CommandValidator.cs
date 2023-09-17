using ClientPricingSystem.Core.MediatRMethods.Client;
using FluentValidation;

namespace ClientPricingSystem.Core.Validators.Client;
public class CreateClient_FromDto_CommandValidator : AbstractValidator<CreateClient_FromDto.Command>
{
    public CreateClient_FromDto_CommandValidator()
    {
        RuleFor(x => x.ClientDto)
            .NotNull()
            .DependentRules(() =>
            {
                RuleFor(x => x.ClientDto.Name)
                    .NotEmpty().NotNull();

                RuleFor(x => x.ClientDto.MarkupRate)
                    .NotEmpty().NotNull();
            });
    }
}