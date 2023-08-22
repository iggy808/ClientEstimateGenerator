using ClientPricingSystem.Core.MediatRMethods.Vendor;
using FluentValidation;

namespace ClientPricingSystem.Core.Validators;
public class CreateVendor_FromDto_CommandValidator : AbstractValidator<CreateVendor_FromDto.Command>
{
    public CreateVendor_FromDto_CommandValidator()
    {
        RuleFor(x => x.VendorDto)
            .NotNull();

        RuleFor(x => x.VendorDto.Name)
            .NotEmpty().NotNull();
    }
}