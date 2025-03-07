using DoaMais.ReportService;
using DoaMais.ReportService.Model.Context;
using DoaMais.ReportService.Repository;
using DoaMais.ReportService.Repository.Interface;
using DoaMais.ReportService.Services;
using DoaMais.ReportService.Services.Interface;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("SqlServer");

builder.Services.AddScoped<IBloodStockRepository, BloodStockRepository>();
builder.Services.AddScoped<IDonationRepository, DonationRepository>();

builder.Services.AddScoped<IReportService, ReportService>();

builder.Services.AddDbContext<SQLServerContext>(opt =>
{
    opt.UseSqlServer(connectionString);
});

// Registra o ServiceScopeFactory para o Worker conseguir criar escopos temporários
builder.Services.AddSingleton<IServiceScopeFactory>(sp => sp.GetRequiredService<IServiceScopeFactory>());

builder.Services.AddHostedService<ReportWorker>();

var host = builder.Build();
host.Run();
