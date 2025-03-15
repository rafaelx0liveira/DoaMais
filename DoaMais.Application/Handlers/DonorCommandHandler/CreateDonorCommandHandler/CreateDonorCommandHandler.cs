using DoaMais.Application.Commands.DonorCommands.CreateDonorCommand;
using DoaMais.Application.Models;
using DoaMais.Domain.Entities;
using DoaMais.Domain.Interfaces.IUnitOfWork;
using MediatR;
using Serilog;

namespace DoaMais.Application.Handlers.DonorCommandHandler.CreateDonorCommandHandler
{
    public class CreateDonorCommandHandler(IUnitOfWork unitOfWork, ILogger logger)
        : IRequestHandler<CreateDonorCommand, ResultViewModel<Guid>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger _logger = logger;

        public async Task<ResultViewModel<Guid>> Handle(CreateDonorCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var donorExists = await _unitOfWork.Donors.DonorExistsAsync(request.Email);

                if (donorExists)
                {
                    _logger.Warning($"Donor with this email already exists: {request.Email}");
                    return ResultViewModel<Guid>.Error("Donor with this email already exists");
                }

                var existingAddress = await _unitOfWork.Address.GetAddressPostalCodeAsync(request.Address.PostalCode);

                Address address;
                if (existingAddress != null)
                {
                    address = existingAddress;
                }
                else
                {
                    address = new Address
                    {
                        StreetAddress = request.Address.StreetAddress,
                        City = request.Address.City,
                        State = request.Address.State,
                        PostalCode = request.Address.PostalCode
                    };

                    await _unitOfWork.Address.AddAddressAsync(address);
                }

                var donor = new Donor
                {
                    Name = request.Name,
                    Email = request.Email,
                    DateOfBirth = request.DateOfBirth,
                    BiologicalSex = request.BiologicalSex,
                    Weight = request.Weight,
                    BloodType = request.BloodType,
                    RHFactor = request.RhFactor,
                    AddressId = address.Id,
                    Address = null
                };

                await _unitOfWork.Donors.AddDonorAsync(donor);
                await _unitOfWork.CompleteAsync();

                _logger.Information($"Donor {donor.Id} created successfully.");
                return ResultViewModel<Guid>.Success(donor.Id);
            }
            catch (Exception ex)
            {
                _logger.Warning($"An error occurred while creating the donor: {ex.Message}");
                return ResultViewModel<Guid>.Error($"One or more errors occurred:{ex.Message}");
            }
        }
    }
}
