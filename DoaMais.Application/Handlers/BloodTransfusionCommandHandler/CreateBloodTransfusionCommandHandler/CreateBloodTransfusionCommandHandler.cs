using DoaMais.Application.Commands.BloodTransfusionCommands.CreateBloodTransfusionCommand;
using DoaMais.Application.Models;
using DoaMais.Domain.Interfaces.IUnitOfWork;
using MediatR;

namespace DoaMais.Application.Handlers.BloodTransfusionCommandHandler.CreateBloodTransfusionCommandHandler
{
    public class CreateBloodTransfusionCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<CreateBloodTransfusionCommand, ResultViewModel<Guid>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<ResultViewModel<Guid>> Handle(CreateBloodTransfusionCommand request, CancellationToken cancellationToken)
        {
            var hospital = await _unitOfWork.Hospital.GetHospitalByCNPJAsync(request.CNPJ);

            if (hospital == null)
                return ResultViewModel<Guid>.Error($"Hospital with CNPJ {request.CNPJ} not found.");
        }
    }
}
