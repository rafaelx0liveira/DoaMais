using DoaMais.Application.Commands.DonationCommands.CreateDonationCommand;
using FluentValidation;

namespace DoaMais.Application.Validators
{
    public class DonationValidator : AbstractValidator<CreateDonationCommand>
    {
        public DonationValidator()
        {
            RuleFor(x => x.QuantityML)
                .InclusiveBetween(420, 470)
                .WithMessage("The amount of blood donated must be between 420ml and 470ml.");
        }
    }
}
