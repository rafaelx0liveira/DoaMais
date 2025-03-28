﻿using DoaMais.Domain.Entities;
using DoaMais.Domain.Interfaces.Repository.BloodTransfusionRepository;
using DoaMais.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace DoaMais.Infrastructure.Repositories.BloodTransfusionRepository
{
    public class BloodTransfusionRepository(SQLServerContext sqlServerContext) : IBloodTransfusionRepository
    {
        private readonly SQLServerContext _sqlServerContext = sqlServerContext;

        public async Task<IEnumerable<BloodTransfusion>> GetAllBloodTransfusionsAsync()
        {
            return await _sqlServerContext.BloodTransfusions
                .Include(bt => bt.Hospital)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
