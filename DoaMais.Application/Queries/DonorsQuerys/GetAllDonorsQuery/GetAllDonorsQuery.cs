using DoaMais.Application.DTOs;
using DoaMais.Application.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoaMais.Application.Queries.DonorsQuerys.GetAllDonorsQuery
{
    public class GetAllDonorsQuery : IRequest<ResultViewModel<IEnumerable<DonorViewModel>>>
    {
    }
}
