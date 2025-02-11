using DoaMais.Application.Handlers.DonorCommandHandler.CreateDonorCommandHandler;
using DoaMais.Application.Services.Auth;
using DoaMais.Application.Services.Auth.Interface;
using DoaMais.Domain.Interfaces.Repository.AddressRepository;
using DoaMais.Domain.Interfaces.Repository.DonorRepository;
using DoaMais.Domain.Interfaces.Repository.EmployeeRepository;
using DoaMais.Domain.Interfaces.UnityOfWork;
using DoaMais.Infrastructure.Context;
using DoaMais.Infrastructure.Persistence;
using DoaMais.Infrastructure.Repositories.AddressRepository;
using DoaMais.Infrastructure.Repositories.DonorRepository;
using DoaMais.Infrastructure.Repositories.EmployeeRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DoaMais.CrossCutting.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("SqlServer");

            services.AddDbContext<SQLServerContext>(opt =>
            {
                opt.UseSqlServer(connectionString);
            });

            // Registrando Repositórios
            services.AddScoped<IDonorRepository, DonorRepository>();
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            services.AddScoped<IAddressRepository, AddressRepository>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ITokenService, TokenService>();

            // Registrando o MediatR para os Handlers dos Commands/Queries
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(CreateDonorCommandHandler).Assembly));

            return services;
        }
    }
}


