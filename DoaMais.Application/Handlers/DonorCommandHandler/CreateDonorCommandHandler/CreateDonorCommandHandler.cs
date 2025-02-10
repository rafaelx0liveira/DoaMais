using DoaMais.Application.Commands.DonorCommands.CreateDonorCommand;
using DoaMais.Application.Models;
using DoaMais.Domain.Interfaces.UnityOfWork;
using MediatR;

namespace DoaMais.Application.Handlers.DonorCommandHandler.CreateDonorCommandHandler
{
    public class CreateDonorCommandHandler(IUnitOfWork unityOfWork)
        : IRequestHandler<CreateDonorCommand, ResultViewModel<Guid>>
    {
        private readonly IUnitOfWork _unityOfWork = unityOfWork;

        public async Task<ResultViewModel<Guid>> Handle(CreateDonorCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var donorExists = await _unityOfWork.Donors.DonorExistsAsync(request.DonorDTO.Email);

                if (donorExists) return ResultViewModel<Guid>.Error("Donor with this email already exists");

                var donor = request.ToEntity();
                await _unityOfWork.Donors.AddDonorAsync(donor);

                return ResultViewModel<Guid>.Success(donor.Id);
            }
            catch (Exception ex)
            {
                return ResultViewModel<Guid>.Error($"One or more errors occurred:{ex.Message}");
            }
        }
    }
}
