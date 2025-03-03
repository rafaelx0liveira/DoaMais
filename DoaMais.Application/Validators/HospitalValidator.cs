using DoaMais.Application.Commands.HospitalCommands.CreateHospitalCommand;
using FluentValidation;
using System.Data;

namespace DoaMais.Application.Validators
{
    public class HospitalValidator : AbstractValidator<CreateHospitalCommand>
    {
        public HospitalValidator() 
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MinimumLength(3).WithMessage("Name must be at least 3 characters long.");

            RuleFor(x => x.CNPJ)
                .NotEmpty().WithMessage("CNPJ is required.")
                .MinimumLength(14).WithMessage("CNPJ must be 14 numbers.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email.");

            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("Phone is required.")
                .MinimumLength(8).WithMessage("Phone must be at least 8 numbers.");
        }
    }
}
