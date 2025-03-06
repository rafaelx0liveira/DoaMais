using DoaMais.HospitalNotificationService;
using DoaMais.MessageBus.Configuration;
using DoaMais.MessageBus.Interface;
using DoaMais.MessageBus;
using Microsoft.EntityFrameworkCore;
using DoaMais.HospitalNotificationService.Model.Context;
using DoaMais.HospitalNotificationService.Repository.Interface;
using DoaMais.HospitalNotificationService.Repository;
using System.Net.Mail;
using System.Net;
using DoaMais.HospitalNotificationService.Services.Interface;
using DoaMais.HospitalNotificationService.Services;

var builder = Host.CreateApplicationBuilder(args);
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

builder.Services.Configure<RabbitMQSettings>(builder.Configuration.GetSection("RabbitMQ"));

builder.Services.AddScoped<IBloodTransfusionRepository, BloodTransfusionRepository>();
builder.Services.AddSingleton<IMessageBus, RabbitMQMessageBus>();
builder.Services.AddSingleton<ISendEmailService, SendEmailService>();

builder.Services.AddDbContext<SQLServerContext>(opt =>
{
    opt.UseSqlServer(connectionString);
});

// Registra o ServiceScopeFactory para o Worker conseguir criar escopos temporários
builder.Services.AddSingleton<IServiceScopeFactory>(sp => sp.GetRequiredService<IServiceScopeFactory>());

var host = builder.Build();
host.Run();
