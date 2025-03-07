using DoaMais.ReportService.Model.Enums;
using Microsoft.EntityFrameworkCore;

namespace DoaMais.ReportService.Model.Context
{
    public class SQLServerContext : DbContext
    {
        public SQLServerContext(DbContextOptions<SQLServerContext> options) : base(options) { }

        public DbSet<BloodStock> BloodStocks { get; set; }
        public DbSet<Donation> Donations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BloodStock>()
                .Property(bs => bs.BloodType)
                .HasConversion(
                    v => v.ToString(),
                    v => (BloodType)Enum.Parse(typeof(BloodType), v)
                );

            modelBuilder.Entity<BloodStock>()
                .Property(bs => bs.RHFactor)
                .HasConversion(
                    v => v.ToString(),
                    v => (RHFactor)Enum.Parse(typeof(RHFactor), v)
                );

            base.OnModelCreating(modelBuilder);
        }
    }
}
