using DoaMais.Application.Commands.DonorCommands.CreateDonorCommand;
using DoaMais.Application.Models;
using DoaMais.Domain.Entities;
using DoaMais.Domain.Interfaces.UnityOfWork;
using MediatR;

namespace DoaMais.Application.Handlers.DonorCommandHandler.CreateDonorCommandHandler
{
    public class CreateDonorCommandHandler(IUnitOfWork unitOfWork)
        : IRequestHandler<CreateDonorCommand, ResultViewModel<Guid>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<ResultViewModel<Guid>> Handle(CreateDonorCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var donorExists = await _unitOfWork.Donors.DonorExistsAsync(request.DonorDTO.Email);

                if (donorExists) return ResultViewModel<Guid>.Error("Donor with this email already exists");

                var existingAddress = await _unitOfWork.Address.GetAddressPostalCodeAsync(request.DonorDTO.Address.PostalCode);

                Address address;
                if (existingAddress != null)
                {
                    address = existingAddress;
                }
                else
                {
                    address = new Address
                    {
                        StreetAddress = request.DonorDTO.Address.StreetAddress,
                        City = request.DonorDTO.Address.City,
                        State = request.DonorDTO.Address.State,
                        PostalCode = request.DonorDTO.Address.PostalCode
                    };

                    await _unitOfWork.Address.AddAddressAsync(address);
                }

                var donor = new Donor
                {
                    Name = request.DonorDTO.Name,
                    Email = request.DonorDTO.Email,
                    DateOfBirth = request.DonorDTO.DateOfBirth,
                    Gender = request.DonorDTO.Gender,
                    Weight = request.DonorDTO.Weight,
                    BloodType = request.DonorDTO.BloodType,
                    RHFactor = request.DonorDTO.RhFactor,
                    AddressId = address.Id,
                    Address = null
                };

                await _unitOfWork.Donors.AddDonorAsync(donor);

                return ResultViewModel<Guid>.Success(donor.Id);
            }
            catch (Exception ex)
            {
                return ResultViewModel<Guid>.Error($"One or more errors occurred:{ex.Message}");
            }
        }
    }
}
