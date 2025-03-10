using DoaMais.MessageBus.Interface;
using DoaMais.MessageBus;
using System.Net.Mail;
using System.Net;
using DoaMais.DonorNotificationService.Services;
using DoaMais.DonorNotificationService.Services.Interface;
using DoaMais.DonorNotificationService;
//using DoaMais.MessageBus.Configuration.Interface;

var builder = Host.CreateApplicationBuilder(args);

//builder.Services.Configure<IRabbitMQConfiguration>(builder.Configuration.GetSection("RabbitMQ"));

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
