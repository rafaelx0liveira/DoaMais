using DoaMais.Application.Commands.DonorCommands.CreateDonorCommand;
using DoaMais.Domain.Entities.Enums;
using FluentValidation;

namespace DoaMais.Application.Validators
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
                .Must(d => d < DateTime.Now.AddYears(-18))
                .WithMessage("Donor has to be over 18.");

            RuleFor(x => x.BiologicalSex)
                .IsInEnum().WithMessage($"Invalid biological sex. Available options: {string.Join(", ", Enum.GetNames(typeof(BiologicalSex)))}.");

            RuleFor(x => x.Weight)
                .GreaterThan(50).WithMessage("Weight must be greater than 50.");

            RuleFor(x => x.BloodType)
                .IsInEnum().WithMessage($"Invalid blood type. Available options: {string.Join(", ", Enum.GetNames(typeof(BloodType)))}.");

            RuleFor(x => x.RhFactor)
                .IsInEnum().WithMessage($"Invalid rh factor. Available options: {string.Join(", ", Enum.GetNames(typeof(RHFactor)))}.");
        }
    }
}
