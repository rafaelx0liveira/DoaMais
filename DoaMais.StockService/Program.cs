using DoaMais.StockService;
using DoaMais.StockService.Model.Context;
using DoaMais.StockService.Repository;
using DoaMais.StockService.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using VaultService.Extensions;
using VaultService.Interface;
using DoaMais.MessageBus.Extensions;

var builder = Host.CreateApplicationBuilder(args);

var vaultAddress = builder.Configuration["KeyVault:Address"] ?? throw new ArgumentNullException("KeyVault Address is missing in Vault");
var vaultToken = builder.Configuration["KeyVault:Token"] ?? throw new ArgumentNullException("KeyVault Token is missing in Vault");

builder.Services.AddVaultService(
    vaultAddress: vaultAddress,
    vaultToken: vaultToken
);

using var serviceProvider = builder.Services.BuildServiceProvider();
var vaultService = serviceProvider.GetRequiredService<IVaultClient>();

var rabbitHost = vaultService.GetSecret(builder.Configuration["KeyVaultSecrets:RabbitMQ:HostName"])
    ?? throw new ArgumentNullException("RabbitMQ HostName is missing");

var rabbitPassword = vaultService.GetSecret(builder.Configuration["KeyVaultSecrets:RabbitMQ:Password"])
    ?? throw new ArgumentNullException("RabbitMQ Password is missing");

var rabbitUserName = vaultService.GetSecret(builder.Configuration["KeyVaultSecrets:RabbitMQ:UserName"])
    ?? throw new ArgumentNullException("RabbitMQ UserName is missing");

builder.Services.AddRabbitMQ(rabbitHost, rabbitUserName, rabbitPassword);

builder.Services.AddHostedService<StockWorker>();

var connectionStringKey = builder.Configuration["KeyVaultSecrets:Database:ConnectionString"] ?? throw new ArgumentNullException("ConnectionString is missing in Vault");
var connectionString = vaultService.GetSecret(connectionStringKey);

builder.Services.AddScoped<IBloodStockRepository, BloodStockRepository>();
builder.Services.AddScoped<IBloodTransfusionRepository, BloodTransfusionRepository>();
builder.Services.AddScoped<IAdminRepository, AdminRepository>();

builder.Services.AddDbContext<SQLServerContext>(opt =>
{
    opt.UseSqlServer(connectionString);
});

// Registra o ServiceScopeFactory para o Worker conseguir criar escopos temporários
builder.Services.AddSingleton<IServiceScopeFactory>(sp => sp.GetRequiredService<IServiceScopeFactory>());

var host = builder.Build();
host.Run();

