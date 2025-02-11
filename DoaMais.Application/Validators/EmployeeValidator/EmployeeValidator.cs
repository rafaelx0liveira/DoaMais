using DoaMais.Application.Commands.EmployeeCommands.CreateEmployeeCommand;
using DoaMais.Domain.Entities.Enums;
using FluentValidation;

namespace DoaMais.Application.Validators.EmployeeValidator
{
    public class EmployeeValidator : AbstractValidator<CreateEmployeeCommand>
    {
        public EmployeeValidator() 
        {
            RuleFor(x => x.Name)
                .NotNull()
                .WithMessage("Name is required.");

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email.");

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Password is required.");

            RuleFor(x => x.Role)
                .IsInEnum().WithMessage($"Invalid role. Available options: {string.Join(", ", Enum.GetNames(typeof(EmployeeRole)))}.");
        }
    }
}
