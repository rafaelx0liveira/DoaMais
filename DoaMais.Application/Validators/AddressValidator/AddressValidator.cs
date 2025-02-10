using DoaMais.Application.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoaMais.Application.Validators.AddressValidator
{
    public class AddressValidator : AbstractValidator<AddressDTO>
    {
        public AddressValidator()
        {
            RuleFor(x => x.StreetAddress)
                .NotEmpty().WithMessage("Logradouro é obrigatório");

            RuleFor(x => x.City)
                .NotEmpty().WithMessage("Cidade é obrigatório");

            RuleFor(x => x.State)
                .NotEmpty().WithMessage("Estado é obrigatório");

            RuleFor(x => x.PostalCode)
                .NotEmpty().WithMessage("CEP é obrigatório");
        }
    }
}
