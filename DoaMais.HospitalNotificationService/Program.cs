using DoaMais.HospitalNotificationService;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Net;
using DoaMais.HospitalNotificationService.Services.Interface;
using DoaMais.HospitalNotificationService.Services;
using VaultService.Extensions;
using VaultService.Interface;
using DoaMais.MessageBus.Extensions;
using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using Serilog;
using Elastic.Serilog.Sinks;

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

// Configuração do Serilog para logging estruturado com o Elasticsearch

// Registrar ElasticsearchClient dinamicamente usando DI
builder.Services.AddSingleton(provider =>
{
    var vaultClient = provider.GetRequiredService<IVaultClient>(); // Resolve o IVaultClient

    var elasticUrlSecret = builder.Configuration["KeyVaultSecrets:Elasticsearch:Url"] ?? throw new ArgumentNullException("Elasticsearch URL is missing in Vault");
    var elasticPasswordSecret = builder.Configuration["KeyVaultSecrets:Elasticsearch:Password"] ?? throw new ArgumentNullException("Elasticsearch Password is missing in Vault");
    var elasticUsernameSecret = builder.Configuration["KeyVaultSecrets:Elasticsearch:Username"] ?? throw new ArgumentNullException("Elasticsearch Username is missing in Vault");

    var elasticUrl = vaultClient.GetSecret(elasticUrlSecret);
    var elasticUsername = vaultClient.GetSecret(elasticUsernameSecret);
    var elasticPassword = vaultClient.GetSecret(elasticPasswordSecret);

    var settings = new ElasticsearchClientSettings(new Uri(elasticUrl))
        .Authentication(new BasicAuthentication(elasticUsername, elasticPassword))
        .ServerCertificateValidationCallback((sender, certificate, chain, sslPolicyErrors) => true);

    return new ElasticsearchClient(settings);
});

builder.Services.AddSerilog((context, config) =>
{
    var elasticClient = builder.Services.BuildServiceProvider().GetRequiredService<ElasticsearchClient>();

    config
        .ReadFrom.Configuration(builder.Configuration)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.File("logs/api_log.txt", rollingInterval: RollingInterval.Day)
        .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(elasticClient.Transport));
});

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
