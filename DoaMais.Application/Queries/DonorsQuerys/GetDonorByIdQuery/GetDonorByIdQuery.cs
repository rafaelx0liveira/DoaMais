using DoaMais.Application.DTOs;
using DoaMais.Application.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoaMais.Application.Queries.DonorsQuerys.GetDonorByIdQuery
{
    public class GetDonorByIdQuery : IRequest<ResultViewModel<DonorViewModel>>
    {
        public GetDonorByIdQuery(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}
