using DoaMais.ReportService;
using DoaMais.ReportService.Model.Context;
using DoaMais.ReportService.Repository;
using DoaMais.ReportService.Repository.Interface;
using DoaMais.ReportService.Services;
using DoaMais.ReportService.Services.Interface;
using Elastic.Clients.Elasticsearch;
using Elastic.Serilog.Sinks;
using Elastic.Transport;
using Microsoft.EntityFrameworkCore;
using Serilog;
using VaultService.Extensions;
using VaultService.Interface;

var builder = Host.CreateApplicationBuilder(args);

using var serviceProvider = builder.Services.BuildServiceProvider();
var vaultService = serviceProvider.GetRequiredService<IVaultClient>();

var vaultAddress = builder.Configuration["KeyVault:Address"] ?? throw new ArgumentNullException("KeyVault Address is missing in Vault");
var vaultToken = builder.Configuration["KeyVault:Token"] ?? throw new ArgumentNullException("KeyVault Token is missing in Vault");

builder.Services.AddVaultService(
    vaultAddress: vaultAddress,
    vaultToken: vaultToken
);

var connectionStringKey = builder.Configuration["KeyVaultSecrets:Database:ConnectionString"] ?? throw new ArgumentNullException("ConnectionString is missing in Vault");
var connectionString = vaultService.GetSecret(connectionStringKey);

builder.Services.AddScoped<IBloodStockRepository, BloodStockRepository>();
builder.Services.AddScoped<IDonationRepository, DonationRepository>();

builder.Services.AddScoped<IReportService, ReportService>();

builder.Services.AddDbContext<SQLServerContext>(opt =>
{
    opt.UseSqlServer(connectionString);
});

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

// Registra o ServiceScopeFactory para o Worker conseguir criar escopos temporários
builder.Services.AddSingleton<IServiceScopeFactory>(sp => sp.GetRequiredService<IServiceScopeFactory>());

builder.Services.AddHostedService<ReportWorker>();

var host = builder.Build();
host.Run();
