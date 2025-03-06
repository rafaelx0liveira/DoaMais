using DoaMais.StockService.Model.Enums;
using Microsoft.EntityFrameworkCore;

namespace DoaMais.StockService.Model.Context
{
    public class SQLServerContext : DbContext
    {
        public SQLServerContext(DbContextOptions<SQLServerContext> options) : base(options) { }

        public DbSet<BloodStock> BloodStocks { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<BloodTransfusion> BloodTransfusions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Garantindo que BloodType e RHFactor sejam únicos em BloodStock
            modelBuilder.Entity<BloodStock>()
                .HasIndex(bs => new { bs.BloodType, bs.RHFactor })
                .IsUnique();

            // Garantindo que QuantityML seja >= 0
            modelBuilder.Entity<BloodStock>()
                        .Property(bs => bs.QuantityML)
                        .HasDefaultValue(0)
                        .IsRequired();
 
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

            modelBuilder.Entity<BloodTransfusion>()
                .Property(bs => bs.BloodType)
                .HasConversion(
                    v => v.ToString(),
                    v => (BloodType)Enum.Parse(typeof(BloodType), v)
                );

            modelBuilder.Entity<BloodTransfusion>()
                .Property(bs => bs.RHFactor)
                .HasConversion(
                    v => v.ToString(),
                    v => (RHFactor)Enum.Parse(typeof(RHFactor), v)
                );

            base.OnModelCreating(modelBuilder);
        }
    }
}
