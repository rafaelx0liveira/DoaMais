using DoaMais.Application.Commands.DonorCommands.CreateDonorCommand;
using DoaMais.Domain.Entities.Enums;
using FluentValidation;

namespace DoaMais.Application.Validators.DonorValidator
{
    public class DonorValidator : AbstractValidator<CreateDonorCommand>
    {
        public DonorValidator() 
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MinimumLength(3).WithMessage("Name must be at least 3 characters long.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email.");

            RuleFor(x => x.DateOfBirth)
                .GreaterThan(DateTime.Today).WithMessage("Date of birth is required.");

            RuleFor(x => x.Gender)
                .IsInEnum().WithMessage($"Invalid gender. Available options: {string.Join(", ", Enum.GetNames(typeof(Gender)))}.");

            RuleFor(x => x.Weight)
                .GreaterThan(0).WithMessage("Weight must be greater than zero.");

            RuleFor(x => x.BloodType)
                .IsInEnum().WithMessage($"Invalid blood type. Available options: {string.Join(", ", Enum.GetNames(typeof(BloodType)))}.");

            RuleFor(x => x.RhFactor)
                .IsInEnum().WithMessage($"Invalid rh factor. Available options: {string.Join(", ", Enum.GetNames(typeof(RHFactor)))}.");
        }
    }
}
