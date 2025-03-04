using DoaMais.Domain.Interfaces.Repository.AddressRepository;
using DoaMais.Domain.Interfaces.Repository.DonorRepository;
using DoaMais.Domain.Interfaces.Repository.EmployeeRepository;
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
using DoaMais.Application.Services.Interface;
using DoaMais.Domain.Interfaces.IUnitOfWork;
using DoaMais.Application.Services.AuthService;
using DoaMais.Domain.Interfaces.Repository.DonationRepository;
using DoaMais.Infrastructure.Repositories.DonationRepository;
using DoaMais.MessageBus.Configuration;
using DoaMais.MessageBus.Interface;
using DoaMais.MessageBus;
using DoaMais.Domain.Interfaces.Repository.HospitalRepository;
using DoaMais.Infrastructure.Repositories.HospitalRepository;
using DoaMais.Domain.Interfaces.Repository.BloodTransfusionRepository;
using DoaMais.Infrastructure.Repositories.BloodTransfusionRepository;

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
                .AddValidation()
                .AddRabbitMQ(configuration);

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
            services.AddScoped<IDonationRepository, DonationRepository>();
            services.AddScoped<IHospitalRepository, HospitalRepository>();
            services.AddScoped<IBloodTransfusionRepository, BloodTransfusionRepository>();

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

        private static IServiceCollection AddRabbitMQ(this IServiceCollection services, IConfiguration configuration)
        {
            // Configuring options for RabbitMQ
            services
                .Configure<RabbitMQSettings>(configuration.GetSection("RabbitMQ"));

            // Registering RabbitMQMessageBus as a Singleton for the entire aplication
            services
                .AddSingleton<IMessageBus, RabbitMQMessageBus>();

            return services;
        }
    }

}


