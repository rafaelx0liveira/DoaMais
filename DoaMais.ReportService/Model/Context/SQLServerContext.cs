using DoaMais.ReportService.Model.Enums;
using Microsoft.EntityFrameworkCore;

namespace DoaMais.ReportService.Model.Context
{
    public class SQLServerContext : DbContext
    {
        public SQLServerContext(DbContextOptions<SQLServerContext> options) : base(options) { }

        public DbSet<BloodStock> BloodStocks { get; set; }
        public DbSet<Donation> Donations { get; set; }
        public DbSet<Donor> Donors { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Report> Reports { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Donor>()
                .Property(d => d.BloodType)
                .HasConversion(
                    v => v.ToString(),
                    v => (BloodType)Enum.Parse(typeof(BloodType), v)
                );

            modelBuilder.Entity<Donor>()
                .Property(d => d.RHFactor)
                .HasConversion(
                    v => v.ToString(),
                    v => (RHFactor)Enum.Parse(typeof(RHFactor), v)
                );

            modelBuilder.Entity<Donor>()
                .Property(d => d.BiologicalSex)
                .HasConversion(
                    v => v.ToString(),
                    v => (BiologicalSex)Enum.Parse(typeof(BiologicalSex), v)
                );

            modelBuilder.Entity<Report>()
                .Property(bs => bs.ReportType)
                .HasConversion(
                    v => v.ToString(),
                    v => (ReportType)Enum.Parse(typeof(ReportType), v)
                );

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
