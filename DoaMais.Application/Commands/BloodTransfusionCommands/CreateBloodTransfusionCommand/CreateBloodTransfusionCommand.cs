using DoaMais.Application.Models;
using MediatR;

namespace DoaMais.Application.Commands.BloodTransfusionCommands.CreateBloodTransfusionCommand
{
    public class CreateBloodTransfusionCommand : IRequest<ResultViewModel<Guid>>
    {
        public string CNPJ { get; }
        public int QuantityML { get; }

        public CreateBloodTransfusionCommand(string cnpj, int quantityML)
        {
            CNPJ = cnpj;
            QuantityML = quantityML;
        }
    }
}
