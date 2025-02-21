using DoaMais.MessageBus;
using DoaMais.MessageBus.Configuration;
using DoaMais.MessageBus.Interface;
using DoaMais.StockService;
using DoaMais.StockService.Model.Context;
using DoaMais.StockService.Repository;
using DoaMais.StockService.Repository.Interface;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<StockWorker>();

var connectionString = builder.Configuration.GetConnectionString("SqlServer");

// Configuração do RabbitMQ
builder.Services.Configure<RabbitMQSettings>(builder.Configuration.GetSection("RabbitMQ"));
builder.Services.AddSingleton<IMessageBus, RabbitMQMessageBus>();

// Adiciona repositórios como Scoped (correto)
builder.Services.AddScoped<IBloodStockRepository, BloodStockRepository>();

// Usa Scoped para o DbContext (correto)
builder.Services.AddDbContext<SQLServerContext>(opt =>
{
    opt.UseSqlServer(connectionString);
});

// Registra o ServiceScopeFactory para o Worker conseguir criar escopos temporários
builder.Services.AddSingleton<IServiceScopeFactory>(sp => sp.GetRequiredService<IServiceScopeFactory>());

var host = builder.Build();
host.Run();

