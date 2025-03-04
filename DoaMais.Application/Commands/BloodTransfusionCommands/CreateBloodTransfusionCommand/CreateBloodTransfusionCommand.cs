using DoaMais.Application.Models;
using DoaMais.Domain.Entities.Enums;
using MediatR;

namespace DoaMais.Application.Commands.BloodTransfusionCommands.CreateBloodTransfusionCommand
{
    public class CreateBloodTransfusionCommand : IRequest<ResultViewModel<Guid>>
    {
        public string CNPJ { get; }
        public int QuantityML { get; }
        public BloodType BloodType { get; }
        public RHFactor RHFactor { get; }

        public CreateBloodTransfusionCommand(
            string cnpj, 
            int quantityML,
            BloodType bloodType,
            RHFactor rHFactor)
        {
            CNPJ = cnpj;
            QuantityML = quantityML;
            BloodType = bloodType;
            RHFactor = rHFactor;
        }
    }
}
