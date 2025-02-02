using Microsoft.EntityFrameworkCore;
using DoaMais.Domain.Entities;
using DoaMais.Domain.Entities.Enums;
using System.Linq.Expressions;

namespace DoaMais.Infrastructure.Context
{
    public class SQLServerContext : DbContext
    {
        public SQLServerContext(DbContextOptions<SQLServerContext> options) : base(options) { }

        public DbSet<Address> Addresses { get; set; }
        public DbSet<BloodStock> BloodStocks { get; set; }
        public DbSet<Donation> Donations { get; set; }
        public DbSet<Donor> Donors { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuração de Enum para string
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

            // Garantindo que BloodType e RHFactor sejam únicos em BloodStock
            modelBuilder.Entity<BloodStock>()
                .HasIndex(bs => new { bs.BloodType, bs.RHFactor })
                .IsUnique();

            // Garantindo que QuantityML seja >= 0
            modelBuilder.Entity<BloodStock>()
                .Property(bs => bs.QuantityML)
                .HasDefaultValue(0)
                .IsRequired();

            // Relacionamento 1:N, Donor -> Donations
            modelBuilder.Entity<Donation>()
                .HasOne(d => d.Donor)
                .WithMany(d => d.Donations)
                .HasForeignKey(d => d.DonorId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relacionamento 1:1, Donor -> Address
            modelBuilder.Entity<Donor>()
                .HasOne(d => d.Address)
                .WithOne()
                .HasForeignKey<Donor>(d => d.AddressId)
                .OnDelete(DeleteBehavior.SetNull);

            base.OnModelCreating(modelBuilder);
        }
    }

}
