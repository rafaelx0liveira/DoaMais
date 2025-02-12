using DoaMais.Application.DTOs;
using FluentValidation;

namespace DoaMais.Application.Validators
{
    public class AddressValidator : AbstractValidator<AddressDTO>
    {
        public AddressValidator()
        {
            RuleFor(x => x.StreetAddress)
                .NotEmpty().WithMessage("StreetAddress is required.");

            RuleFor(x => x.City)
                .NotEmpty().WithMessage("City is required.");

            RuleFor(x => x.State)
                .NotEmpty().WithMessage("State is required.");

            RuleFor(x => x.PostalCode)
                .NotEmpty().WithMessage("Postal code is required.");
        }
    }
}
