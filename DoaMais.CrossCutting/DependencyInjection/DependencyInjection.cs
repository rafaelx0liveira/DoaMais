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
using DoaMais.MessageBus.Interface;
using DoaMais.MessageBus;
using DoaMais.Domain.Interfaces.Repository.HospitalRepository;
using DoaMais.Infrastructure.Repositories.HospitalRepository;
using DoaMais.MessageBus.Configuration;
using VaultService.Extensions;
using VaultService.Interface;
using DoaMais.MessageBus.Extensions;

namespace DoaMais.CrossCutting.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services
                .AddVaultService(configuration)
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
            services.AddDbContext<SQLServerContext>((provider, options) =>
            {
                var vaultService = provider.GetRequiredService<IVaultClient>();

                var connectionStringKey = configuration["KeyVaultSecrets:Database:ConnectionString"] ?? throw new ArgumentNullException("ConnectionString is missing in Vault");
                var connectionString = vaultService.GetSecret(connectionStringKey);

                options.UseSqlServer(connectionString);
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
            using var serviceProvider = services.BuildServiceProvider();
            var vaultService = serviceProvider.GetRequiredService<IVaultClient>();

            var rabbitHost = vaultService.GetSecret(configuration["KeyVaultSecrets:RabbitMQ:HostName"])
                ?? throw new ArgumentNullException("RabbitMQ HostName is missing");

            var rabbitPassword = vaultService.GetSecret(configuration["KeyVaultSecrets:RabbitMQ:Password"])
                ?? throw new ArgumentNullException("RabbitMQ Password is missing");

            var rabbitUserName = vaultService.GetSecret(configuration["KeyVaultSecrets:RabbitMQ:UserName"])
                ?? throw new ArgumentNullException("RabbitMQ UserName is missing");

            services.AddRabbitMQ(rabbitHost, rabbitUserName, rabbitPassword);

            return services;
        }

        private static IServiceCollection AddVaultService(this IServiceCollection services, IConfiguration configuration) 
        {
            var vaultAddress = configuration["KeyVault:Address"] ?? throw new ArgumentNullException("KeyVault Address is missing"); ;
            var vaultToken = configuration["KeyVault:Token"] ?? throw new ArgumentNullException("KeyVault Token is missing"); ;

            return services.AddVaultService(
                vaultAddress: vaultAddress,
                vaultToken: vaultToken
            );
        }
    }

}


