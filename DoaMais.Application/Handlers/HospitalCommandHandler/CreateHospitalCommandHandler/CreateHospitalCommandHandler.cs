using DoaMais.Application.Commands.HospitalCommands.CreateHospitalCommand;
using DoaMais.Application.Models;
using DoaMais.Domain.Entities;
using DoaMais.Domain.Interfaces.IUnitOfWork;
using MediatR;

namespace DoaMais.Application.Handlers.HospitalCommandHandler.CreateHospitalCommandHandler
{
    public class CreateHospitalCommandHandler (IUnitOfWork unitOfWork) : IRequestHandler<CreateHospitalCommand, ResultViewModel<Guid>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        public async Task<ResultViewModel<Guid>> Handle(CreateHospitalCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var hospitalExists = await _unitOfWork.Hospital.HospitalExistsAsync(request.CNPJ);

                if(hospitalExists) return ResultViewModel<Guid>.Error($"Hospital with CNPJ {request.CNPJ} already exists");

                var hospital = new Hospital
                {
                    Name = request.Name,
                    CNPJ = request.CNPJ,
                    Email = request.Email,
                    Phone = request.Phone,
                    IsActive = true
                }; 

                await _unitOfWork.Hospital.AddHospitalAsync(hospital);

                return ResultViewModel<Guid>.Success(hospital.Id);
            }
            catch (Exception ex)
            {
                return ResultViewModel<Guid>.Error($"One or more errors occurred: {ex.Message}");
            }
        }
    }
}
