using DoaMais.Application.DTOs;
using DoaMais.Application.Models;
using DoaMais.Application.Queries.DonorsQuerys.GetAllDonorsQuery;
using DoaMais.Domain.Interfaces.UnityOfWork;
using MediatR;

namespace DoaMais.Application.Handlers.DonorQueryHandler.GetAllDonorsQueryHandler
{
    public class GetAllDonorsQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetAllDonorsQuery, ResultViewModel<IEnumerable<DonorDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<ResultViewModel<IEnumerable<DonorDTO>>> Handle(GetAllDonorsQuery request, CancellationToken cancellationToken)
        {
            var donors = await _unitOfWork.Donors.GetAllDonorsAsync();

            AddressDTO addressDTO;

            var donorDTOs = donors.Select(x => new DonorDTO
            {
                Name = x.Name,
                Email = x.Email,
                DateOfBirth = x.DateOfBirth,
                Gender = x.Gender,
                Weight = x.Weight,
                BloodType = x.BloodType,
                RhFactor = x.RHFactor,
                Address = new AddressDTO
                {
                    StreetAddress = x.Address.StreetAddress,
                    City = x.Address.City,
                    State = x.Address.State,
                    PostalCode = x.Address.PostalCode
                }
            }).ToList();

            return ResultViewModel<IEnumerable<DonorDTO>>.Success(donorDTOs);
        }
    }
}
