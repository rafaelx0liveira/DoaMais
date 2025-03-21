using DoaMais.ReportService.Model;
using DoaMais.ReportService.Model.Context;
using DoaMais.ReportService.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoaMais.ReportService.Repository
{
    public class ReportRepository(SQLServerContext context)
        : IReportRepository
    {
        private readonly SQLServerContext _context = context;

        public async Task AddAsync(Report report)
        {
            await _context.Reports.AddAsync(report);
            await _context.SaveChangesAsync();
        }
    }
}
