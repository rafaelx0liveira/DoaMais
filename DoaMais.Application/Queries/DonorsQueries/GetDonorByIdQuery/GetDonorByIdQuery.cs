using DoaMais.Application.DTOs;
using DoaMais.Application.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoaMais.Application.Queries.DonorsQueries.GetDonorByIdQuery
{
    public class GetDonorByIdQuery : IRequest<ResultViewModel<DonorViewModel>>
    {
        public Guid Id { get; set; }

        public GetDonorByIdQuery(Guid id)
        {
            Id = id;
        }

    }
}
