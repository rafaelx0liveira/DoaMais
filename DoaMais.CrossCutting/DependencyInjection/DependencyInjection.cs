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
using Microsoft.Extensions.Options;
using DoaMais.Infrastructure.Services;
using DoaMais.MessageBus.Configuration;
using DoaMais.Application.Interface;

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
                var vaultService = provider.GetRequiredService<IKeyVaultService>();

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
            services.AddSingleton<RabbitMQSettings>(provider =>
            {
                var vaultService = provider.GetRequiredService<IKeyVaultService>();

                // Pegando as secrets do Vault com base no appsettings.json
                var rabbitHost = vaultService.GetSecret(configuration["KeyVaultSecrets:RabbitMQ:HostName"])
                    ?? throw new ArgumentNullException("RabbitMQ HostName is missing");

                var rabbitPassword = vaultService.GetSecret(configuration["KeyVaultSecrets:RabbitMQ:Password"])
                    ?? throw new ArgumentNullException("RabbitMQ Password is missing");

                var rabbitUserName = vaultService.GetSecret(configuration["KeyVaultSecrets:RabbitMQ:UserName"])
                    ?? throw new ArgumentNullException("RabbitMQ UserName is missing");

                return new RabbitMQSettings
                {
                    HostName = rabbitHost,
                    Password = rabbitPassword,
                    UserName = rabbitUserName
                };
            });

            services.AddSingleton<IMessageBus>(provider =>
            {
                var settings = provider.GetRequiredService<RabbitMQSettings>();
                return new RabbitMQMessageBus(settings);
            });

            return services;
        }

        private static IServiceCollection AddVaultService(this IServiceCollection services, IConfiguration configuration) 
        {
            return services.AddSingleton<IKeyVaultService, KeyVaultService>();
        }
    }

}


