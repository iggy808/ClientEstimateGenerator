using ClientPricingSystem.Core.Methods.Vendor;
using FluentValidation;

namespace ClientPricingSystem.Core.Validators.Vendor;
public class CreateVendor_FromDto_CommandValidator : AbstractValidator<CreateVendor_FromDto.Command>
{
    public CreateVendor_FromDto_CommandValidator()
    {
        RuleFor(x => x.VendorDto)
            .NotNull()
            .DependentRules(() =>
            {
                RuleFor(x => x.VendorDto.Name)
                    .NotEmpty().NotNull();
            });
    }
}