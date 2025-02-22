using DoaMais.DonorNotificationWorker;
using DoaMais.DonorNotificationWorker.Services;
using DoaMais.DonorNotificationWorker.Services.Interface;
using DoaMais.MessageBus.Interface;
using DoaMais.MessageBus;
using System.Net.Mail;
using System.Net;
using DoaMais.MessageBus.Configuration;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<RabbitMQSettings>(builder.Configuration.GetSection("RabbitMQ"));

builder.Configuration.AddEnvironmentVariables(prefix: "SENDGRID_");
builder.Services.AddSingleton<ISendEmailService, SendEmailService>();
builder.Services.AddSingleton<IMessageBus, RabbitMQMessageBus>();

builder.Services.AddHostedService<DonorNotificationWorker>();

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


var host = builder.Build();
host.Run();
