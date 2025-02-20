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
        public DbSet<Employee> Employees { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Relacionamento 1:N Address -> Donor
            modelBuilder.Entity<Donor>()
                .HasOne(d => d.Address) // Donor has one address
                .WithMany(a => a.Donors) // Address has many donors
                .HasForeignKey(d => d.AddressId);

            // Relacionamento 1:N, Donor -> Donations
            modelBuilder.Entity<Donation>()
                .HasOne(d => d.Donor)
                .WithMany(d => d.Donations)
                .HasForeignKey(d => d.DonorId)
                .OnDelete(DeleteBehavior.Restrict);

            //Relacionamento 1:N, Address -> Employee
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Address)
                .WithMany(e => e.Employees)
                .HasForeignKey(e => e.AddressId)
                .OnDelete(DeleteBehavior.Restrict);

            // Garantindo que BloodType e RHFactor sejam únicos em BloodStock
            modelBuilder.Entity<BloodStock>()
                .HasIndex(bs => new { bs.BloodType, bs.RHFactor })
                .IsUnique();

            modelBuilder.Entity<Donor>()
                .HasIndex(e => e.Email)
                .IsUnique();

            // Garantindo que QuantityML seja >= 0
            modelBuilder.Entity<BloodStock>()
                .Property(bs => bs.QuantityML)
                .HasDefaultValue(0)
                .IsRequired();

            modelBuilder.Entity<Employee>()
                .HasIndex(e => e.Email)
                .IsUnique();

            // Configuração de Enum para string
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
                .Property(d => d.BiologicalSex)
                .HasConversion(
                    v => v.ToString(),
                    v => (BiologicalSex)Enum.Parse(typeof(BiologicalSex), v)
                );

            modelBuilder.Entity<Employee>()
                .Property(d => d.Role)
                .HasConversion(
                    v => v.ToString(),
                    v => (EmployeeRole)Enum.Parse(typeof(EmployeeRole), v)
                );

            base.OnModelCreating(modelBuilder);
        }
    }

}
