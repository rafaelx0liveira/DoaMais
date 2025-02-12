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
using FluentValidation;
using DoaMais.Application.Commands.EmployeeCommands.CreateEmployeeCommand;
using FluentValidation.AspNetCore;

namespace DoaMais.CrossCutting.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services
                .AddDatabase(configuration)
                .AddRepositories()
                .AddUnitOfWork()
                .AddServices()
                .AddHandlers()
                .AddValidation();

            return services;
        }

        private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("SqlServer");

            services.AddDbContext<SQLServerContext>(opt =>
            {
                opt.UseSqlServer(connectionString);
            });

            return services;
        }

        private static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IDonorRepository, DonorRepository>();
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            services.AddScoped<IAddressRepository, AddressRepository>();

            return services;
        }

        private static IServiceCollection AddUnitOfWork(this IServiceCollection services)
        {
            return services.AddScoped<IUnitOfWork, UnitOfWork>();
        }

        private static IServiceCollection AddServices(this  IServiceCollection services)
        {
            return services.AddScoped<ITokenService, TokenService>();
        }

        private static IServiceCollection AddHandlers(this IServiceCollection services)
        {
            return services.AddMediatR(cfg => 
                cfg.RegisterServicesFromAssemblyContaining<CreateEmployeeCommand>());
        }

        private static IServiceCollection AddValidation(this IServiceCollection services)
        {
            return services
                .AddFluentValidationAutoValidation()
                .AddValidatorsFromAssemblyContaining<CreateEmployeeCommand>();
        }
    }

}


