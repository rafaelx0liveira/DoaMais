using DoaMais.Application.DTOs;
using FluentValidation;

namespace DoaMais.Application.Validators.DonorValidator
{
    public class DonorValidator : AbstractValidator<DonorDTO>
    {
        public DonorValidator() 
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Nome é obrigatório.")
                .MinimumLength(3).WithMessage("Nome deve ter pelo menos 3 caracteres.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email é obrigatório")
                .EmailAddress().WithMessage("Email inválido");

            RuleFor(x => x.Weight)
                .GreaterThan(0).WithMessage("Peso deve ser maior que zero.");

            RuleFor(x => x.DateOfBirth)
                .GreaterThan(DateTime.Today).WithMessage("Data de nascimento inválida");
        }
    }
}
