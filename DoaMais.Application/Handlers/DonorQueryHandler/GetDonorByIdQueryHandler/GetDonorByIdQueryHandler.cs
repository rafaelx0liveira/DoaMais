using DoaMais.Application.DTOs;
using DoaMais.Application.Models;
using DoaMais.Application.Queries.DonorsQuerys.GetDonorByIdQuery;
using DoaMais.Domain.Interfaces.UnityOfWork;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoaMais.Application.Handlers.DonorQueryHandler.GetDonorByIdQueryHandler
{
    public class GetDonorByIdQueryHandler (IUnitOfWork unitOfWork) : IRequestHandler<GetDonorByIdQuery, ResultViewModel<DonorViewModel>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public Task<ResultViewModel<DonorViewModel>> Handle(GetDonorByIdQuery request, CancellationToken cancellationToken)
        {
            var donor = _unitOfWork.Donors.GetDonorByIdAsync(request.Id);

            throw new NotImplementedException();
        }
    }
}
