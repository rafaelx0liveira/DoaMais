using DoaMais.HospitalNotificationService;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Net;
using DoaMais.HospitalNotificationService.Services.Interface;
using DoaMais.HospitalNotificationService.Services;
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

builder.Services.AddHostedService<HospitalWorker>();

string? sendGridKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY", EnvironmentVariableTarget.User);

builder.Services.AddScoped<SmtpClient>(provider =>
{
    var smtpClient = new SmtpClient("smtp.sendgrid.net")
    {
        Port = 587,
        Credentials = new NetworkCredential("apikey", sendGridKey),
        EnableSsl = true
    };
    return smtpClient;
});

var connectionString = builder.Configuration.GetConnectionString("SqlServer");

builder.Services.AddSingleton<ISendEmailService, SendEmailService>();

// Registra o ServiceScopeFactory para o Worker conseguir criar escopos temporários
builder.Services.AddSingleton<IServiceScopeFactory>(sp => sp.GetRequiredService<IServiceScopeFactory>());

var host = builder.Build();
host.Run();
